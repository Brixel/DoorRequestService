import { Injectable } from '@angular/core';

import {
   HttpInterceptor,
   HttpRequest,
   HttpHandler,
   HttpEvent,
} from '@angular/common/http';

import { UserService } from './user.service';

import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
   constructor(private userService: UserService) {}

   intercept(
      req: HttpRequest<any>,
      next: HttpHandler
   ): Observable<HttpEvent<any>> {
      if (this.userService.isAuthenticated()) {
         const accessToken = JSON.parse(this.userService.getToken())
            .access_token;
         const token = `Bearer ${accessToken}`;
         const authReq = req.clone({
            headers: req.headers.set('Authorization', token),
         });
         return next.handle(authReq) as any;
      }

      return next.handle(req) as any;
   }
}
