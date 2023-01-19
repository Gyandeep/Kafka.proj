import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-topic-stats',
  templateUrl: './topic-stats.component.html',
  styleUrls: ['./topic-stats.component.css']
})
export class TopicStatsComponent implements OnInit {

  httpClient: HttpClient | null = null;
  url = '';
  public stats: any = null;
  public partitions: any = [];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpClient = http;
    this.url = baseUrl;
  }

  ngOnInit(): void {
  }

  onGetStats() {
    this.httpClient?.get(this.url + 'admin/topic-stats').subscribe(result => {
      debugger
      var str = result ? result.toString() : "";
      this.stats = JSON.parse(str);
      for (let key in this.stats.topics) {
        
        let value = this.stats.topics[key];
        debugger
        this.partitions[value.topic] = value.partitions[0];
        // Use `key` and `value`
    }
    }, error => console.error(error));
  }

}
