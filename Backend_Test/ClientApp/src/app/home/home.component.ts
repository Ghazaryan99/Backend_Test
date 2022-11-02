import { Component, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http'
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent {

  size: string = '';
  BlurEffect: boolean = false;
  GrayscaleEffect: boolean = false;
  effect3: boolean = false;
  radius: string = '';
  base64textString = [];
  selectedFile: File = null;

  result: any;

  constructor(
    private http: HttpClient,
    private _sanitizer: DomSanitizer,
    @Inject('BASE_URL')
    private baseUrl: string) {
  }

  onFileSelected(event) {
    this.selectedFile = <File>event.target.files[0];
    const fileData = new FormData();
    fileData.append('image', this.selectedFile, this.selectedFile.name)
    this.http.post('https://localhost:5001/api/Values', fileData).subscribe(result => {
      console.log(result);
    }, error => console.error(error));
  }

  clickFile(event) {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();

      reader.onload = this.handleReaderLoaded.bind(this);
      reader.readAsBinaryString(file);
    }
  }

  handleReaderLoaded(e) {
    this.base64textString.push('data:image/png;base64,' + btoa(e.target.result));
  }

  Apply() {
    const params = new HttpParams()
      .set('size', this.size.toString())
      .set('effect1', this.BlurEffect.toString())
      .set('effect2', this.GrayscaleEffect.toString())
      .set('effect3', this.effect3.toString())
      .set('radius', this.radius.toString())

    this.http.get<any>(this.baseUrl + 'api/Values', { params }).subscribe(result => {
      this.result = this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpg;base64,'
        + result);
    }, error => console.error(error));
  }
}
