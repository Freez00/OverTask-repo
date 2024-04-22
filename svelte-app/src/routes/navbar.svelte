<script lang="ts">
    import { onMount } from 'svelte';
    import {goto} from '$app/navigation';
    import {isAuthenticated} from '$lib/scripts/requestHandler'
    import {writable} from "svelte/store";
    import {LogOut} from '$lib/scripts/authHandler';
    import {getUsername} from '$lib/scripts/requestHandler';




    let isMobile = false;
    let dropdownOpen = false;
    let isLoggedIn = writable<boolean>(false);

    let username = 'Няма връзка със сървъра';

    $: if ($isLoggedIn) {
        getUsername().then(result => {
            username = result;
        });
    }

    onMount(async () => {
        // Check if the device is mobile
        isMobile = window.innerWidth <= 768;
        isLoggedIn.set(await isAuthenticated());

    });

    async function handleLogOut(){
        await LogOut();
        let status = await isAuthenticated();
        isLoggedIn.set(status);
        console.log("just updated status and it is");
        console.log(status)

        await goto('/authentication');
    }

    function toggleMobileMenu() {
        isMobile = !isMobile;
    }

    function navigateTo(path:string) {
        window.location.href = path;
    }

    function handleClickOutside(event:any) {
        // Check if the click was outside the dropdown
        if (dropdownOpen && !document.querySelector('.dropdown')?.contains(event.target)) {
            dropdownOpen = false;
        }
    }

</script>

<svelte:window on:click={handleClickOutside} />


<nav class="navbar navbar-expand-lg navbar-dark bg-dark navbar-custom">
    <a class="navbar-brand" href="/">OverTask</a>
    <button
        class="navbar-toggler"
        type="button"
        aria-controls="navbarNav"
        aria-expanded="false"
        aria-label="Toggle navigation"
        on:click={toggleMobileMenu}
    >
        <span class="navbar-toggler-icon"></span>
    </button>
    <div
        class="collapse navbar-collapse justify-content-between"
        id="navbarNav"
        class:show="{isMobile}"
    >
        <ul class="navbar-nav">
            <li class="nav-item">
                <a class="nav-link" href="../todo">Задачи</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="../calendar/" on:click|preventDefault={() => navigateTo('../calendar/')}>Календар</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="../groups">Групи</a>
            </li>
            {#if !$isLoggedIn}
                <li class="nav-item d-lg-none">
                    <a class="nav-link" href="../authentication">Влизане</a>
                </li>
            {:else}
                <li class="nav-item d-lg-none">
                    <a class="nav-link" href="#">{username}</a>
                </li>
                <li class="nav-item d-lg-none">
                    <a class="nav-link" href="#" on:click={handleLogOut}>Излизане</a>
                </li>
            {/if}
        </ul>
    </div>
    <ul class="navbar-nav user-nav d-none d-lg-flex">
        {#if !$isLoggedIn}
            <li class="nav-item">
                <a class="nav-link" href="../authentication">Влизане</a>
            </li>
        {:else}
            <li class="nav-item dropdown">
                <a class="nav-link" href="#" on:click|preventDefault="{() => dropdownOpen = !dropdownOpen}">
                    {username}
                </a>
                {#if dropdownOpen}
                    <div class="dropdown-menu show">
                        <a class="dropdown-item" href="#">Профил</a>
                        <a class="dropdown-item" href="#" on:click={handleLogOut}>Излизане</a>
                    </div>
                {/if}
            </li>
        {/if}
    </ul>
</nav>

<style>
    .show1 {
        display: flex !important;
    }
    .navbar-custom{
        background-color: #8864b5 !important;
    }
    nav *{
        font-size: large;
    }
    .navbar-collapse {
        justify-content: space-between;
    }
    .dropdown-menu {
        position: absolute;
        top: 100%;
        right: 0;
        left: auto;
        width: auto;
        transform: none;
    }
    .navbar-nav {
        padding-right: 20px;
    }
    .navbar-brand{
        margin-left: 20px;
		font-family: "Anton", sans-serif !important;
    }
</style>