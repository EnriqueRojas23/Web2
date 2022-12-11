import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { AuthUtils } from 'app/core/auth/auth.utils';
import { UserService } from 'app/core/user/user.service';
import { environment } from 'environments/environment';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
    providedIn: 'root'
  })

export class AuthService
{
    public jwtHelper = new JwtHelperService();
    private _authenticated = false;

    private baseUrl = environment.baseUrl + '/api/auth/';
    private decodedToken: any;
    private menu?: any[];
    /**
     * Constructor
     */
    constructor(
        private _httpClient: HttpClient,
        private _userService: UserService
    )
    {
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Accessors
    // -----------------------------------------------------------------------------------------------------

    /**
     * Setter & getter for access token
     */
    set accessToken(token: string)
    {
        localStorage.setItem('token', token);
    }

    get accessToken(): string
    {
        return localStorage.getItem('token') ?? '';
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Forgot password
     *
     * @param email
     */
    forgotPassword(email: string): Observable<any>
    {
        // eslint-disable-next-line no-underscore-dangle
        return this._httpClient.post('api/auth/forgot-password', email);
    }

    /**
     * Reset password
     *
     * @param password
     */
    resetPassword(password: string): Observable<any>
    {
        // eslint-disable-next-line no-underscore-dangle
        return this._httpClient.post('api/auth/reset-password', password);
    }

    /**
     * Sign in
     *
     * @param credentials
     */
    signIn(credentials: { username: string; password: string }): Observable<any>
    {
        // Throw error, if the user is already logged in
        // eslint-disable-next-line no-underscore-dangle
        if ( this._authenticated )
        {
            return throwError('User is already logged in.');
        }

        // eslint-disable-next-line no-underscore-dangle
        return this._httpClient.post( this.baseUrl + 'login', credentials).pipe(
            switchMap((response: any) => {

                const user = response;


                // Se elimina el local storage
                localStorage.clear();

                   //Get and Set menú
                const stringMenu = JSON.stringify(user.menu);
                localStorage.setItem('menu', stringMenu);

                localStorage.setItem('token', user.token);
                this.decodedToken = this.jwtHelper.decodeToken(user.token);

                const esCliente = JSON.stringify(user.rol_id);


                localStorage.setItem('escliente', esCliente);


                // Store the access token in the local storage
                this.accessToken = response.token;

                // Set the authenticated flag to true
                // eslint-disable-next-line no-underscore-dangle
                this._authenticated = true;

                // Store the user on the user service
                // eslint-disable-next-line no-underscore-dangle
                this._userService.user = response.user;

                // Return a new observable with the response
                return of(response);
            })
        );
    }

    /**
     * Sign in using the access token
     */
    signInUsingToken(): Observable<any>
    {
        // Renew token
        return of(true);
        // return this._httpClient.post('api/auth/refresh-access-token', {
        //     accessToken: this.accessToken
        // }).pipe(
        //     catchError(() =>

        //         // Return false
        //         of(false)
        //     ),
        //     switchMap((response: any) => {

        //         // Store the access token in the local storage
        //         this.accessToken = response.accessToken;

        //         // Set the authenticated flag to true
        //         this._authenticated = true;

        //         // Store the user on the user service
        //         this._userService.user = response.user;

        //         // Return true
        //         return of(true);
        //     })
        // );
    }

    /**
     * Sign out
     */
    signOut(): Observable<any>
    {
        // Remove the access token from the local storage
        localStorage.removeItem('token');

        // Set the authenticated flag to false
        // eslint-disable-next-line no-underscore-dangle
        this._authenticated = false;

        // Return the observable
        return of(true);
    }

    /**
     * Sign up
     *
     * @param user
     */
    signUp(user: { name: string; email: string; password: string; company: string }): Observable<any>
    {
        // eslint-disable-next-line no-underscore-dangle
        return this._httpClient.post('api/auth/sign-up', user);
    }

    /**
     * Unlock session
     *
     * @param credentials
     */
    unlockSession(credentials: { email: string; password: string }): Observable<any>
    {
        // eslint-disable-next-line no-underscore-dangle
        return this._httpClient.post('api/auth/unlock-session', credentials);
    }

    /**
     * Check the authentication status
     */
    check(): Observable<boolean>
    {
        // Check if the user is logged in
        // eslint-disable-next-line no-underscore-dangle
        if ( this._authenticated )
        {
            return of(true);
        }

        // Check the access token availability
        if ( !this.accessToken )
        {
            return of(false);
        }

        // Check the access token expire date
        if ( AuthUtils.isTokenExpired(this.accessToken) )
        {
            return of(false);
        }

        // If the access token exists and it didn't expire, sign in using it
        return this.signInUsingToken();
    }
}
