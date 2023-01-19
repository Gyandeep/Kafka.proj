import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
  selector: 'app-topic-creation',
  templateUrl: './topic-creation.component.html',
  styleUrls: ['./topic-creation.component.css']
})
export class TopicCreationComponent implements OnInit {
  profileForm = new FormGroup({
    topicName : new FormControl(''),
    partitions: new FormControl(''),
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
    console.warn(this.profileForm.value);

    var request =  {
      topicName: this.profileForm.value.topicName,
      numberOfPartitions: parseInt(this.profileForm.value.partitions ?? "1")
    }

    if (request.topicName == '' || request.numberOfPartitions == 0) {
      this.message = "Validation errors.";
      this.alertType = "warning"
        this.showAlert = true;
      return;
    }

    this.httpClient?.post(this.url + 'admin/topic-create', JSON.stringify(request), { headers: {'Content-Type':'application/json; charset=utf-8'  } }).subscribe(result => {
        console.log("Created topic");
        this.alertType = "success";
        this.message = "Created topic successfully!!";
        this.showAlert = true;

        // this.profileForm.value.topicName = '';
        // this.profileForm.value.partitions = "0";
    }, error => console.error(error));
  }
  
}