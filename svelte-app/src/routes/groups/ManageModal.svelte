<script lang='ts'>
	import type { Group } from '$lib/scripts/groupHandler';
    import { onMount } from 'svelte';

	export let showModal:Boolean; // boolean
	export let buttonAction:() => Promise<void|Boolean>; 
	export let deleteButtonAction: (() => Promise<void|Boolean>)|undefined = undefined;
	export let buttonTitle:string;
	export let badMessage:string;
	export let managedGroup:Group|undefined = undefined;
	export let wantTopLine: Boolean | undefined = undefined;

	let originalName:string;
	let originalPictureURL:string;

	let dialog:HTMLDialogElement; // HTMLDialogElement

	let errorMessage = '';

	$: if (dialog && showModal) dialog.showModal();

	onMount(async () => {
		if(managedGroup){
			originalName = managedGroup.Name;
			originalPictureURL = managedGroup.PictureURL;
		}
	})

	async function handleAction(){
		let response = await buttonAction();
		if (response){
			errorMessage = '';
			showModal = false;
			dialog.close();
		}
		else{
			errorMessage = badMessage;
		}
	}
</script>

<!-- svelte-ignore a11y-click-events-have-key-events a11y-no-noninteractive-element-interactions -->
<dialog
	bind:this={dialog}
	on:close={() =>{
		if (managedGroup){
			managedGroup.Name =originalName;
			managedGroup.PictureURL = originalPictureURL;
		}
			showModal = false;
			errorMessage = '';
		}
	}
	on:click|self={() => {
			dialog.close();
			if (managedGroup){
			managedGroup.Name =originalName;
			managedGroup.PictureURL = originalPictureURL;
			}
			errorMessage = '';
		}
	}
>
	<!-- svelte-ignore a11y-no-static-element-interactions -->
	<div on:click|stopPropagation>
		<slot name="header" />
		{#if wantTopLine || wantTopLine === true}
			<hr />
		{/if}
		<slot />
		{#if deleteButtonAction}
		<hr>
		<div class="mb-3">
			<button class="delete-button" on:click={async () => {if(deleteButtonAction){await deleteButtonAction();} showModal=false; dialog.close();}}><b>Delete Group</b></button>
		</div>
		{/if}
		<hr />
		<p style="color:red;position:absolute;bottom:50px;"><b>{errorMessage}</b></p>
		<div class="button-holder">
			<button on:click={() => dialog.close()}>Затвори</button>
			<button class="action-button" on:click={async () => { await handleAction()}}>{buttonTitle}</button>
		</div>
	</div>
</dialog>

<style>
	dialog {
		max-width: 32em;
        min-width: 350px;
		border-radius: 0.2em;
		border: none;
		padding: 0;
		font-family: "Cabin", sans-serif;
		font-optical-sizing: auto;
		font-weight: 400;
		font-style: normal;
		font-variation-settings:
			"wdth" 100;
	}
	dialog::backdrop {
		background: rgba(0, 0, 0, 0.3);
	}
	dialog > div {
		padding: 1em;
	}
	dialog[open] {
		animation: zoom 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
	}
	@keyframes zoom {
		from {
			transform: scale(0.95);
		}
		to {
			transform: scale(1);
		}
	}
	dialog[open]::backdrop {
		animation: fade 0.2s ease-out;
	}
	@keyframes fade {
		from {
			opacity: 0;
		}
		to {
			opacity: 1;
		}
	}
	button {
		display: block;
        background-color: white;
        color: #333;
        padding: 5px 10px;
        border-radius: 3px;
        cursor: pointer;
        margin-top:5px;
        border: 2px solid #373c4f;
    }
	button:hover {
		background-color: rgb(237, 237, 237);
	}
	.action-button {
        background-color: darkorchid;
        color: white;
        padding: 5px 10px;
        border-radius: 3px;
        cursor: pointer;
        margin-top:5px;
        border: 2px solid #373c4f;
    }

	.action-button:hover {
		background-color: rgb(181, 115, 212);
	}

	.button-holder {
		margin-top:30px;
		display: flex;
		justify-content: space-between;
	}

	.button-holder button {
		min-width: 100px;
	}

	.delete-button{
        background-color:#E04B5A; 
        color: #fff;
        width:100%; 
        border:0px; 
        border-radius:20px; 
        min-height: 40px;
        transition: 0.5s ease all;
    }
    .delete-button:hover{
        background-color:#cecece; 
        color: darkorchid;
    }
</style>
