<script lang='ts'>
	export let showModal:Boolean; // boolean
	export let buttonAction:() => Promise<void|Boolean>; // 
	export let buttonTitle:string;
	export let badMessage:string;

	let dialog:HTMLDialogElement; // HTMLDialogElement

	let errorMessage = '';

	$: if (dialog && showModal) dialog.showModal();

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
		showModal = false;
		errorMessage = '';
		}
	}
	on:click|self={() => {
		dialog.close()
		errorMessage = '';
		}
	}
>
	<!-- svelte-ignore a11y-no-static-element-interactions -->
	<div on:click|stopPropagation>
		<slot name="header" />
		<hr />
		<slot />
		<hr />
		<!-- svelte-ignore a11y-autofocus -->
		<p style="color:red;position:absolute;bottom:50px; font-size:small;"><b>{errorMessage}</b></p>
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
</style>
