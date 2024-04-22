import { backendURL } from "./variables";
import { getToken } from "./requestHandler";

export interface AuthResource{
    Email:string,
    Password:string,
    UserName: string,
}


export async function Login(model:AuthResource){

    const response = await fetch(`${backendURL}/authenticate/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(model)
      });
      
    return response;
}

export async function Register(model:AuthResource){
  const response = await fetch(`${backendURL}/authenticate/register`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(model)
  })
  return response;
}

export async function LogOut(){
  console.log('Logging out');
  const response = await fetch(`${backendURL}/authenticate/logout`, {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${await getToken()}`,
      'Content-Type': 'application/json'
    }
  });
  document.cookie = 'token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
}