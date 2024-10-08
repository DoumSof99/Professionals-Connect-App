import { Component, inject, OnInit } from '@angular/core';
import { MemberService } from '../../_services/member.service';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { MemberCardComponent } from "../member-card/member-card.component";

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [MemberCardComponent, PaginationModule],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css'
})
export class MemberListComponent implements OnInit{
  memberService = inject(MemberService);
  pageNumber = 1;
  pageSize = 5;
  
  ngOnInit(): void {
    if(!this.memberService.paginatedResult()) this.loadMembers();
  }

  loadMembers(){
    this.memberService.getMembers(this.pageNumber, this.pageSize);
  }

  pageChanged(event: any){
    if(this.pageNumber !== event.page){
      this.pageNumber = event.page;
      this.loadMembers();
    }
  }

}
