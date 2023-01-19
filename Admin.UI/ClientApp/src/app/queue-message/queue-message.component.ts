import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
  selector: 'app-queue-message',
  templateUrl: './queue-message.component.html',
  styleUrls: ['./queue-message.component.css']
})
export class QueueMessageComponent implements OnInit {

  queueMessageForm = new FormGroup({
    scheduleAs: new FormControl(''),
    count: new FormControl(''),
  });

  httpClient: HttpClient | null = null;
  url = '';
  showAlert: boolean = false;
  message: string = "";
  alertType: string = "success";

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpClient = http;
    this.url = baseUrl;
  }

  ngOnInit(): void {
  }

  onSubmit() {
    // TODO: Use EventEmitter with form value
    console.warn(this.queueMessageForm.value);

    var request = {
      key: "key1",
      message:"This is a message",
      messagesCount: this.queueMessageForm.value.count
    }

    // if (request.topicName == '' || request.numberOfPartitions == 0) {
    //   this.message = "Validation errors.";
    //   this.alertType = "warning"
    //   this.showAlert = true;
    //   return;
    // }

    let relativeUrl: string = "";

    if (this.queueMessageForm.value.scheduleAs == "sameday") {
      relativeUrl = "admin/schedulesamedaymessages";
    }
    else if (this.queueMessageForm.value.scheduleAs == "future") {
      relativeUrl = "admin/schedulefuturemessages";
    }
    else {
      relativeUrl = "admin/queueimmediatemessages"
    }


    this.httpClient?.post(this.url + relativeUrl, JSON.stringify(request), { headers: { 'Content-Type': 'application/json; charset=utf-8' } }).subscribe(result => {
      console.log("Scheduled the message");
      this.alertType = "success";
      this.message = "Message scheduled successfully!!";
      this.showAlert = true;

      // this.queueMessageForm.value.scheduleAs = '';
      // this.queueMessageForm.value.count = "0";
    }, error => console.error(error));
  }
}
