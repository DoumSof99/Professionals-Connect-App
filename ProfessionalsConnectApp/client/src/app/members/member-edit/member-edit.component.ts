import { Component, HostListener, inject, OnInit, ViewChild } from '@angular/core';
import { Member } from '../../_models/member';
import { AccountService } from '../../_services/account.service';
import { MemberService } from '../../_services/member.service';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-member-edit',
  standalone: true,
  imports: [TabsModule, FormsModule],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css'
})
export class MemberEditComponent implements OnInit{
  @ViewChild('editForm') editForm?: NgForm;
  @HostListener('window:beforeunload', ['$event']) notify($event:any) {
    if(this.editForm?.dirty){
      $event.returnValue = true;
    }
  }

  member?: Member;
  private accountServive = inject(AccountService);
  private memberServive = inject(MemberService);
  private toastr = inject(ToastrService);

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember(){
    const user = this.accountServive.currentUser();
    if(!user) return;
    this.memberServive.getMember(user.username).subscribe({
      next: member => this.member = member
    });
  }

  updateMember(){
    console.log(this.member);
    this.toastr.success('Profile updated succesfully');
    this.editForm?.reset(this.member);
  }

}
