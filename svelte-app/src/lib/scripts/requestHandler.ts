import { json } from "@sveltejs/kit";
import { browser } from "$app/environment";
import { onMount } from 'svelte';
import {goto} from '$app/navigation';
import { backendURL } from "./variables";



export async function getToken() {
    let cookie;
    if (browser) {
        cookie = document.cookie.split('; ').find(row => row.startsWith('token'));
    }
    if (!cookie) {
        console.log('Token not found');
        return;
    }

    const token = cookie.split('=')[1];
    return token;
}   

export async function getUserId(){
    const response = await fetch(`${backendURL}/account/getUserId`, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${await getToken()}`,
            'Content-Type': 'application/json'
        }
    });
    const data = await response.json();
    return data.userID;
}

export async function isAuthenticated(){
    if(await isServerRunning() == false){
        await removeToken();
        return false;
    }
    if(document.cookie.match(/^(.*;)?\s*token\s*=\s*[^;]+(.*)?$/) != null){
        return true;
      }
      else{
        return false;
      }
}

export async function getUsername(){
    const response = await fetch(`${backendURL}/account/getUsername`, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${await getToken()}`,
            'Content-Type': 'application/json'
        }
    });
    
    if(response.ok)
    {
        const data = await response.json();
        let username = data.username;
        return username;
    }

    return "Unknown";
}

async function isServerRunning(){
    try {
        const response = await fetch(`${backendURL}/account/serverStatus`, {
            method: 'GET',
        });
        return response.ok;
    } catch (error) {
        console.error('Failed to fetch server status:', error);
        return false;
    }
}

async function removeToken(){
    if (browser) {
        document.cookie = 'token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
    }
}