import { Http, Response, Headers, RequestOptionsArgs } from '@angular/http';

export class HttpServiceBase {
  private baseUrl: string = 'https://configstorage-api.azurewebsites.net/api';

  constructor(private _http: Http) { }

  protected async request<T>(url: string, data: any, headers?: { [name: string]: any }): Promise<T> {
    try {
      const options: RequestOptionsArgs = {};
      if (headers) {
        options.headers = new Headers(headers);
      }
      const response = await this._http.post(`${this.baseUrl}/${url}`, data, options).toPromise();
      return <T>response.json();
    } catch (e) {
      const errorResponse = <Response>e;
      console.log(errorResponse.json());
      throw e;
    }
  }

  protected async requestVoid(url: string, data: any, headers?: { [name: string]: any }): Promise<void> {
    try {
      const options: RequestOptionsArgs = {};
      if (headers) {
        options.headers = new Headers(headers);
      }
      await this._http.post(`${this.baseUrl}/${url}`, data, options).toPromise();
    } catch (e) {
      const errorResponse = <Response>e;
      console.log(errorResponse.json());
      throw e;
    }
  }
}
