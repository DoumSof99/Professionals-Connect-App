using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using SQLitePCL;

namespace API.SignalR;

public class MessageHub(IMessageRespository messageRespository, IUserRepository userRepository, IMapper mapper) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["user"];

        if(Context.User == null || string.IsNullOrEmpty(otherUser)) throw new Exception("Cannot join group");
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);        
        await AddToGroup(groupName);

        var messages = await messageRespository.GetMessageThread(Context.User.GetUsername(), otherUser!);
        await Clients.Group(groupName).SendAsync("RecievedMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await RemoveFromMessageGroup();
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto){
        var username = Context.User?.GetUsername() ?? throw new Exception("Chould not get user");
        if(username == createMessageDto.RecipientUsername.ToLower()) throw new Exception("You cannot message yourself");

        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if(recipient == null ||sender == null || sender.UserName == null || recipient.UserName == null) 
            throw new Exception("Cannot send message at this time");

        var message = new Message{
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await messageRespository.GetMessageGroup(groupName);

        if(group != null && group.Connections.Any(x => x.Username == recipient.UserName)){
            message.DateRead = DateTime.UtcNow;
        }

        messageRespository.AdMessage(message);

        if(await messageRespository.SaveAllAsync()) {
            await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
        }

    }

    private async Task<bool> AddToGroup(string groupName){
        var username = Context.User?.GetUsername() ?? throw new Exception("Cannot get username");
        var group = await messageRespository.GetMessageGroup(groupName);
        var connection = new Connection { ConnectionId = Context.ConnectionId, Username = username };

        if(group == null){
            group = new Group { Name = groupName};
            messageRespository.AddGroup(group);
        }
        group.Connections.Add(connection);
        return await messageRespository.SaveAllAsync();
    }

    private async Task RemoveFromMessageGroup() {
        var connection = await messageRespository.GetConnection(Context.ConnectionId);
        if(connection != null){
            messageRespository.RemoveConnection(connection);
            await messageRespository.SaveAllAsync();
        }
    }

    private string GetGroupName(string caller, string? other){
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}
