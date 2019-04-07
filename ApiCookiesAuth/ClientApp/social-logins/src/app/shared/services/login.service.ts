import {Inject, Injectable} from '@angular/core';
import {environment} from '../../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {DOCUMENT} from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  constructor(@Inject(DOCUMENT) private document: Document,
              private http: HttpClient) {
  }

  loginGoogle() {
    this.document.location.href = `${environment.APIEndpoint}/api/auth/signInWithGoogle`;
    // return this.http.get(`${environment.APIEndpoint}/api/auth/signInWithGoogle`)
    //   .pipe(map((response: any) => {
    //     console.log(response);
    //   }));
  }

  logout() {
    this.http.post(`/account/logout`, {}).subscribe(_ => {
      // redirect the user to a page that does not require authentication
      console.log('logout');
    });
  }
}
