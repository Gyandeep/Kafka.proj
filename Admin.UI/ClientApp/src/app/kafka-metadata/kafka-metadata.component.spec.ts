import { ComponentFixture, TestBed } from '@angular/core/testing';

import { KafkaMetadataComponent } from './kafka-metadata.component';

describe('KafkaMetadataComponent', () => {
  let component: KafkaMetadataComponent;
  let fixture: ComponentFixture<KafkaMetadataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ KafkaMetadataComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(KafkaMetadataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
