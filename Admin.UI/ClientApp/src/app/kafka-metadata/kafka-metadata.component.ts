import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-kafka-metadata',
  templateUrl: './kafka-metadata.component.html',
  styleUrls: ['./kafka-metadata.component.css']
})
export class KafkaMetadataComponent implements OnInit {

  httpClient: HttpClient | null = null;
  url = '';
  public metadata: any = null;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpClient = http;
    this.url = baseUrl;
   }

  ngOnInit(): void {
  }

  onGetMetadata() {
    this.httpClient?.get(this.url + 'admin/metadata').subscribe(result => {
      this.metadata = result;
  }, error => console.error(error));
  }
}
