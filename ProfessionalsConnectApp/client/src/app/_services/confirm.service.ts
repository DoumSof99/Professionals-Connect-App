import { inject, Injectable } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  bsModelRef?: BsModalRef;
  private modalService = inject(BsModalService);

  confirm(
    title = 'Confirmation',
    message = 'Are you sure you want to do this?',
    btnOkText = 'Ok',
    btnCancelText = 'Cancel'
  ){
    const config: ModalOptions = {
      initialState: {
        title, 
        message,
        btnOkText,
        btnCancelText
      }
    };
    this.bsModelRef = this.modalService.show('confirm', config);
  }
  
}
