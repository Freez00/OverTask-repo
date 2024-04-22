<script lang="ts">
    import { onMount } from 'svelte';
    import { fade, scale } from 'svelte/transition';
    import {spring} from 'svelte/motion';
    import Navbar from '../navbar.svelte';
    import {Register} from '$lib/scripts/authHandler';
    import {Login} from '$lib/scripts/authHandler';
    import {goto} from '$app/navigation';
    import { writable } from 'svelte/store';

    let isRegister = false;
    let email = '';
    let username = '';
    let password = '';
    let confirmPassword = '';
    
    let errorMessages = writable<string[]>([]);

    const passwordRegex = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*]).{8,}$/;
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    
    $: model = {
        UserName: username,
        Email:email,
        Password:password
    }
    $: isRegister, errorMessages.set([]);

    let buttonColor = '#7f54b4'; // initial color
    function handleMouseDown() {
        buttonColor = '#5f2c9e'; // flash color
    }
    function handleMouseUp() {
        buttonColor = '#7f54b4'; // original color
    }
    function handleMouseEnter(){
        buttonColor = '#8864b5'; // flash color
    }
    
    async function handleSubmit() {
        errorMessages.update(messages => []);
        if (isRegister) {
            await handleRegister();
        } else {
            await handleLogin();
        }
    }

    async function handleRegister(){
        if(!(email === '' || username === '' || password === '') && 
        passwordRegex.test(password) && emailRegex.test(email) &&
         password === confirmPassword)
        {
            Register(model).then(response =>{
                if(response.ok){
                    response.json().then(data =>{
                        document.cookie = `token=${data.token};path=/;Secure;SameSite=Strict;`
                        goto('/');
                    });
                }
                else{
                    errorMessages.update(messages => [...messages, "User with provided name/email already exists."]);
                }
            }).catch(_ => {
                errorMessages.update(_ => ["Unable to connect to the server. Please check your internet connection and try again."])
            });
        }
        else{
            if(!emailRegex.test(email)){
                errorMessages.update(messages => [...messages, "Email must be valid."])
            }
            if(!passwordRegex.test(password)){
                errorMessages.update(messages => [...messages, "Password must be at least 8 characters long and contain 1 lowercase, 1 uppercase, 1 number, 1 special character."]);
            }
            if(!(password === confirmPassword)){
                errorMessages.update(messages => [...messages, "Passwords must match."])
            }
            if(email === '' || username === '' || password === ''){
                errorMessages.update(_ => ["All fields are required."])
            }
        }
    }

    async function handleLogin(){
        if(!(email === '' || password === '')){
            Login(model).then(response => {
                if(response.ok){
                    response.json().then(data => {
                    document.cookie = `token=${data.token};path=/;Secure;SameSite=Strict;`;
                    goto('/')
                    });
                }
                else
                {                
                    errorMessages.update(_ => ["User with the provided information was not found."])
                }
            }).catch(_ => {
                errorMessages.update(_ => ["Unable to connect to the server. Please check your internet connection and try again."])
            });
        }
        else{
            errorMessages.update(_ => ["All fields are required."])
        }
    }

    onMount(() => {
        // Add any additional initialization logic here
    });

    function slide(node: HTMLElement, { delay = 0, duration = 400 }) {
        const style = getComputedStyle(node);
        const opacity = +style.opacity;
        const height = parseFloat(style.height);
        const padding_top = parseFloat(style.paddingTop);
        const padding_bottom = parseFloat(style.paddingBottom);
        const margin_top = parseFloat(style.marginTop);
        const margin_bottom = parseFloat(style.marginBottom);
        const border_top_width = parseFloat(style.borderTopWidth);
        const border_bottom_width = parseFloat(style.borderBottomWidth);

        return {
            delay,
            duration,
            css: (t: number) => `
                opacity: ${t * opacity};
                height: ${t * height}px;
                padding-top: ${t * padding_top}px;
                padding-bottom: ${t * padding_bottom}px;
                margin-top: ${t * margin_top}px;
                margin-bottom: ${t * margin_bottom}px;
                border-top-width: ${t * border_top_width}px;
                border-bottom-width: ${t * border_bottom_width}px;
            `
        };
    }
</script>

<div class="all">

    <Navbar />
    <main>
        <div class="container mx-auto">
            <div class="title-container">
                <h1 class="title" class:active={!isRegister} on:click={() => isRegister = false}>Вход</h1>
                <h1 class="title" class:active={isRegister} on:click={() => isRegister = true}>Регисър</h1>
            </div>
            <form on:submit|preventDefault={handleSubmit}>
                {#if isRegister}
                <div class="mb-3" transition:slide={{}}>
                    <label for="username" class="form-label">Потребителско име</label>
                    <input type="text" class="form-control" id="username" bind:value={username}  />
                </div>
                {/if}
                <div class="mb-3" transition:slide={{}}>
                    <label for="email" class="form-label">Имейл</label>
                    <input type="text" class="form-control" id="email" bind:value={email}  />
                </div>
                <div class="mb-3" transition:slide={{}}>
                    <label for="password" class="form-label">Парола</label>
                    <input type="password" class="form-control" id="password" bind:value={password}  />
                </div>
                {#if isRegister}
                <div class="mb-3" transition:slide={{}}>
                    <label for="confirmPassword" class="form-label">Потвърждение на паролата</label>
                    <input type="password" class="form-control" id="confirmPassword" bind:value={confirmPassword}  />
                </div>
                {/if}
                <div class="mb-3">
                    <ul style="color: #E04B5A;">
                        {#each $errorMessages as message (message)}
                            <li>{message}</li>
                        {/each}
                    </ul>
                </div>
                <div class="mt-1 d-flex justify-content-center">
                    <button type="submit" class="btn btn-primary" style="background-color:{buttonColor}" 
                        transition:scale on:mousedown={handleMouseDown} on:mouseup={handleMouseUp} on:mouseleave={handleMouseUp}
                        on:mouseenter={handleMouseEnter}>
                        {isRegister ? 'Регистриране' : 'Влизане'}
                    </button>
                </div>
            </form>
        </div>
</main>
</div>

<style>

    body, html, main{
        height:73%;
        margin: 0;
        display: flex;
        justify-content: center;
        align-items: center;
    }


    .all{
        height:100vh;
        background-image: linear-gradient(transparent, #7f54b4);
        /*background-image: url('login-bg.jpg');*/
        

    }
    
    .container {
    max-width: 400px; /* This might not be necessary if you're using a Bootstrap container */
    margin: auto auto;
    padding: 20px;
    background-color: #373c4f;
    border-radius: 20px;
    font-family: "Cabin", sans-serif;
		font-optical-sizing: auto;
		font-weight: 400;
		font-style: normal;
		font-variation-settings:
			"wdth" 100;
    
    /*height:524px;*/
}

.title-container {
    display: flex;
    justify-content: space-between;
    margin-bottom: 20px;
}

    .title {
        text-align: center;
        font-size: 24px;
        cursor: pointer;
        transition: all 0.3s;
        color:#b0abcf;
    }

    .title.active {
        color: rgb(151, 151, 255);
        font-size: 30px;
    }

    .form-label {
        font-weight: bold;
        color:#fff;
    }

    .form-control {
        width: 100%;
        padding: 10px;
        margin-bottom: 10px;
        border: 1px solid #b0abcf;
        border-radius: 7px;
    }

    .btn {
        padding: 10px 20px;
        border-radius: 4px;
        cursor: pointer;
    }

    .btn-primary {
        background-color: #7f54b4;
        color: #fff;
        border: none;
    }
    
</style>