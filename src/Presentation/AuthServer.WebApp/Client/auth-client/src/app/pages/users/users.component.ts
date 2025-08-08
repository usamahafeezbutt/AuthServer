import { Component, OnInit, ViewChild } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { AccountDetailDto, AccountDetailDtoListPagedListResponse, BaseResponse, Client, UpdateAccountStatusDto } from '../../shared/webapi/client';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators, } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { RippleModule } from 'primeng/ripple';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { RatingModule } from 'primeng/rating';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { SelectModule } from 'primeng/select';
import { RadioButtonModule } from 'primeng/radiobutton';
import { InputNumberModule } from 'primeng/inputnumber';
import { DialogModule } from 'primeng/dialog';
import { TagModule } from 'primeng/tag';
import { InputIconModule } from 'primeng/inputicon';
import { IconFieldModule } from 'primeng/iconfield';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../shared/services/auth.service';
import { SelectButtonModule } from 'primeng/selectbutton';
import { PasswordModule } from 'primeng/password';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css'],
  standalone: true,
    imports: [
        CommonModule,
        RouterModule,
        TableModule,
        FormsModule,
        ReactiveFormsModule,
        ButtonModule,
        RippleModule,
        ToastModule,
        ToolbarModule,
        RatingModule,
        InputTextModule,
        TextareaModule,
        SelectModule,
        SelectButtonModule,
        RadioButtonModule,
        InputNumberModule,
        DialogModule,
        TagModule,
        InputIconModule,
        IconFieldModule,
        ConfirmDialogModule,
        PasswordModule
    ],
  providers: [MessageService, ConfirmationService, Client, DialogService]
})
export class UsersComponent implements OnInit {
  
  @ViewChild('dt') dt!: Table;
  userDialog: boolean = false;
  users: AccountDetailDto[];
  cols: any;
  selectedUsers!: AccountDetailDto[] | null;
  addUpdateUserForm: FormGroup;
  user: AccountDetailDto;
  submitted: boolean = false;
  totalRecords: number;
  pageSize: number = 6;
  pageNumber: number = 0;
  isUseradmin: boolean = false;
  roles = [
            { name: 'Admin' },
            { name: 'Staff' },
            { name: 'User'}
        ];
  stateOptions: any[] = [{ label: 'Active', value: 1 },{ label: 'Inactive', value: 2 }];


  constructor(
    private formBuilder: FormBuilder,
    private clientService: Client,
    public dialogService: DialogService,
    private authService: AuthService,
    private messageService: MessageService) { }

  ngOnInit() {
    this.createAddUpdateUserForm();
    this.getUsers();

    this.cols = [
      { field: 'status', header: 'Status' },
      { field: 'name', header: 'Name' },
      { field: 'email', header: 'Email' },
      { field: 'userName', header: 'User Name' },
      { field: 'phoneNumber', header: 'Phone No' },
    ];

    this.isUseradmin = this.authService.hasRole('Admin');
  }

  createAddUpdateUserForm(user: AccountDetailDto = null){
    this.addUpdateUserForm = this.formBuilder.group({
      id:[this.user ? this.user.id : undefined],
      password: [],
      name: [this.user ? this.user.name : '', Validators.required],
      email: [this.user ? this.user.email : '', Validators.required],
      phoneNumber: [this.user ? this.user.phoneNumber : ''],
      status: [this.user ? this.user.status : ''],
        role: [this.user ? { name: this.user.role } : '']
    });
  }

  getUsers(){
    this.clientService.getAccountDetails(
      '',
      false,
      this.pageNumber,
      this.pageSize).subscribe({
      next: (response: AccountDetailDtoListPagedListResponse) => {
        if(response.success){
          this.users = response.result;
        }else{
          if(response.totalRecords > 0 && this.pageNumber > 0)
          {
            this.pageNumber = this.pageNumber - 1;
            this.getUsers();
          }
          this.users = [];
        }
      }
    });
  }

  updateAccountStatus(accountDetail: AccountDetailDto, status: number){
    let updateAccountStatusDto = new UpdateAccountStatusDto();
    updateAccountStatusDto.accountId = accountDetail.id;
    updateAccountStatusDto.status = status;

    this.clientService.updateAccountStatus(updateAccountStatusDto).subscribe(
      {
        next: (response : BaseResponse)=> {
        if(response){
          if(response.success){
            this.messageService.add({ severity: 'info', summary: 'Success', detail: "Account Status Updated Successfully" });
            this.getUsers();
          }else{
            this.messageService.add({ severity: 'info', summary: 'Failed', detail: "Account Status not Updated, please try again." });
          }
        }else{
          this.messageService.add({ severity: 'info', summary: 'Failed', detail: "Account Status not Updated, please try again." });
        }
      },
      error: () => this.messageService.add({ severity: 'info', summary: 'Failed', detail: "Account Status not Updated, please try again." })
    });
  }

  openNew() {
    this.user = new AccountDetailDto();
    this.submitted = false;
    this.userDialog = true;
    this.createAddUpdateUserForm();
  }

  editProduct(user: AccountDetailDto) {
    this.user = user;
    this.userDialog = true;
    this.createAddUpdateUserForm(this.user);
  }

  addUpdateUser(){
    if(this.user.id){
      let user = this.addUpdateUserForm.value;
      user.userName = this.addUpdateUserForm.value.email;
      user.role = this.addUpdateUserForm.value.role.name;
      this.clientService.updateAccountDetails(user).subscribe(
        {
          next: (response : BaseResponse)=> {
            if(response){
              if(response.success){
                this.messageService.add({ severity: 'info', summary: 'Success', detail: "Account Updated Successfully" });
                this.getUsers();
              }else{
                this.messageService.add({ severity: 'info', summary: 'Failed', detail: "Account not Updated, please try again." });
              }
            }else{
              this.messageService.add({ severity: 'info', summary: 'Failed', detail: "Account not Updated, please try again." });
            }
        },
        error: () => this.messageService.add({ severity: 'info', summary: 'Failed', detail: "Account not Updated, please try again." })
      });
    }else{
      let user = this.addUpdateUserForm.value;
      user.userName = this.addUpdateUserForm.value.email;
      user.role = this.addUpdateUserForm.value.role.name;

      this.clientService.register(user).subscribe(
        {
          next: (response : BaseResponse)=> {
            if(response){
              if(response.success){
                this.messageService.add({ severity: 'info', summary: 'Success', detail: "Account added Successfully" });
                this.getUsers();
              }else{
                this.messageService.add({ severity: 'info', summary: 'Failed', detail: "Account not added, please try again." });
              }
            }else{
              this.messageService.add({ severity: 'info', summary: 'Failed', detail: "Account not added, please try again." });
            }
        },
        error: () => this.messageService.add({ severity: 'info', summary: 'Failed', detail: "Account not added, please try again." })
      });
    }

    this.userDialog = false;
    this.user = null;

  }

  exportCSV() {
    this.dt.exportCSV();
  }

  onGlobalFilter(table: Table, event: Event) {
    table.filterGlobal((event.target as HTMLInputElement).value, 'contains');
  }
}