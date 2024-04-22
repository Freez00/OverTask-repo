<script lang="ts">
    import {writable} from 'svelte/store';
    import {tick} from 'svelte';
    import {goto} from '$app/navigation';
    import {onMount} from 'svelte';
    
    import {createTaskAPI, deleteTaskAPI, updateTaskAPI} from '$lib/scripts/todoHandler';
    import {deleteCategoryAPI, updateCategoryAPI} from '$lib/scripts/todoHandler';

    import {createTaskLocal, deleteTaskLocal, updateTaskLocal} from '$lib/scripts/todoHandler';
    import {deleteCategoryLocal, updateCategoryLocal} from '$lib/scripts/todoHandler';


    import {slide } from 'svelte/transition';
    import {isAuthenticated} from '$lib/scripts/requestHandler';



    export let record:any;
    export let editingCategory:any;
    export let editingTask:any;
    export let getInfo:any;
    
    let inputElement:any;
    let createInput:any;
    let title:string;
    let originalTitle:string;
    let color:string = "#fff";
    let categoryID:number;
    let colorPicker:any;
    let isCreatingTask:boolean = false;
    let dropdownActive:boolean = true;

    $:isEditingCategory = record.Category.Id === editingCategory; 

    $: {    
        if (isEditingCategory) {
            tick().then(() => {
                inputElement.focus();
            });
        }
    }

    $: {
        if (isCreatingTask) {
            tick().then(() => {
                createInput.focus();
            });
        }
    }

    onMount(async () => {
        categoryID = record.Category.Id;
        console.log("The category ID is: " + categoryID + " and the title is: " + record.Category.Title);
    });

    async function startEditing() {
        isCreatingTask=false;
        $editingCategory =record.Category.Id;
        originalTitle = record.Category.Title;
        title = record.Category.Title; 
        isEditingCategory=true;
    }

    async function confirmEdit() {
        if(await isAuthenticated()){
            await updateCategoryAPI(categoryID, title, colorPicker.value);
        } else {
            updateCategoryLocal(categoryID, title, colorPicker.value);
        }

        $editingCategory = -2;
        isEditingCategory = false;
        await getInfo();
    }

    function cancelEdit(){
        $editingCategory = -2;
        isEditingCategory = false;
        title = originalTitle;
    }

    function startCreating(){
        title = '';
        isEditingCategory = false;
        dropdownActive = true;
        isCreatingTask = true;
        
    }

    async function confirmCreate(){
        if(await isAuthenticated()){
            await createTaskAPI(categoryID, title);
        } else {
            createTaskLocal(categoryID, title);
        }
        await getInfo();
        title = ''; 
        isCreatingTask=false;
    }


    async function handleDeleteCategoryAPI() {
        if(await isAuthenticated()){
            await deleteCategoryAPI(categoryID);
        } else {
            deleteCategoryLocal(categoryID);
        }
        await getInfo();
    }

    async function handleDeleteTaskAPI(taskID: number) {
        if(await isAuthenticated()){
            await deleteTaskAPI(taskID);
        } else {
            deleteTaskLocal(taskID);
        }
        await getInfo();
    }

    async function handleUpdateTaskAPI(taskID: number, title:string, completed: boolean) {
        if(await isAuthenticated()){
            await updateTaskAPI(taskID, title, completed);
        } else {
            updateTaskLocal(taskID, title, completed);
        }
        await getInfo();
    }


</script>

<div class="category-section">
    <div class="category">
        {#if isEditingCategory}
            <input class="" type="text" bind:value={title} bind:this={inputElement} placeholder={record.Category.Title}/>
            <input type="color" id="color-picker" bind:this={colorPicker}>
            <div class="category-actions">
                <button on:click={async () => {await confirmEdit()}}>Потвърждаване</button>
                <button on:click={() => {cancelEdit()}}>Отказване</button>
            </div>
        {:else}
        <div style="display:inline-flex; justify-content:center; align-items:center;vertical-align:middle;">
            <button style="margin-left:0.5rem" class="dropdown-button" on:click={() => {dropdownActive = !dropdownActive; isCreatingTask=false;}}>
                <i class="fa fa-caret-right" class:rotate={dropdownActive}></i></button>
            <h1 style="align: left;">{record.Category.Title}</h1>
        </div>
        {#if record.Category.Id != -1}
        <div class="category-actions">
            <button on:click={()=> {startCreating()}}>+</button>
            <button on:click={async () => {await startEditing()}}>Редакция на категория</button>
            <button on:click={async () => {await handleDeleteCategoryAPI()}}>Изтриване на категория</button>
        </div>
        {:else}
        <div class="category-actions">
            <button on:click={()=> {startCreating()}}>+</button>
        </div>
        {/if}

        {/if}
    </div>
    {#if dropdownActive}
    <div class="dropdown-content" class:active={dropdownActive} transition:slide={{duration:500}}>
        {#each record.Tasks as task}
        <div class={`task ${task.Completed ? 'crossed-out' : ''}`}>
            <div class="task-actions">
                <input id="{task.Id}" type="checkbox" checked={task.Completed} on:change={async() => {await handleUpdateTaskAPI(task.Id, task.Title, !task.Completed)}}/>
                <p>{task.Title}</p>
            </div>
            <div class="task-actions">
                <button on:click={async () => {await handleDeleteTaskAPI(task.Id)}}>Изтриване</button>
            </div>
        </div>
        {/each}
    </div>
    {/if}
    {#if isCreatingTask}

    <div class="task">
        <input type="text" bind:value={title} bind:this={createInput} placeholder="Въведете задача"/>
        <div class="task-actions">
            <button on:click={async () => {confirmCreate()}}>Създаване</button>
            <button on:click={async () => {isCreatingTask=false; title='';}}>Отказване</button>
        </div>
    </div>
    {/if}
</div>


<style>
    .header{
        margin-top:2rem;
        font-size:4rem;
        display: flex;
        justify-content:center;
        align-items: center;
    }
    .anton-regular {
        font-family: "Anton", sans-serif;
        font-weight: 400;
        font-style: normal;
    }

    .category-section {
        align-items: left;
        display: block;
        width:70%;
        background-color: rgba(138, 122, 160, 0.5);
        margin-bottom:2rem;
        border-radius: 15px;
        overflow: hidden;
        
    }

    @media only screen and (max-width: 600px){
        .category-section{
            width: 100%;
        }
        
    }

    

    .category{
        display: flex;
        justify-content: space-between;
        align-items: center;
        vertical-align: middle;

        background-color: #8B7AA0;
        
    }
    .category input, .category h1{
        /*height:3rem;*/
        padding:0;
        margin:0;
        margin-left:1rem; 
        vertical-align: middle;
    }

    .category input{
        font-size: 2rem;
        width:100%;
        border:0;
        background-color: transparent;
    }
    .category input:focus{
        border:0;
        background-color: transparent;
    }

    #color-picker {
        -webkit-appearance: none;
        -moz-appearance: none;
        appearance: none;
        width: 2.5rem;
        height: 2.5rem;
        background-color: transparent;
        cursor: pointer;
        border: 1px solid white;
        margin: 5px;
    }
    #color-picker::-webkit-color-swatch {
        border: 1px solid white;
        border: none;
    }
    #color-picker::-moz-color-swatch {
        border: 1px solid white;
        border: none;
    }

    .task{
        display: flex;
        justify-content: space-between;
        align-items: center;
        position: relative;
        
    }

    .task *{
        height: 100%;
        vertical-align: middle;
        justify-content: center;
        align-items: center;
        margin-left: 2rem;

    }
    .task-text{
        max-width: 80%;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;

    }
    .task p{
        margin: 0;
        margin-left: 0.5rem;
    }
    .task-actions{
        display: inline-flex;
        z-index: 2;
        margin-right:0.3rem;
        
    }


    .category-actions{
        display: inline-flex;
        margin-right:1rem;
    }

    .categories-container{
        /*word-wrap: normal;*/
    }

    .crossed-out{
        color: #523875; /* Change this to the color you want */
        background-color: rgba(0, 0, 0, 0.13);

    }
    .crossed-out::after {
        content: "";
        position: absolute;
        top: 50%;
        width: 100%;
        
        border: 1px solid #523875; /* Change the color as needed */
        
    }

    .rotate{
        transform: rotate(90deg);            
    }
    .fa-caret-right{
            transition: transform 0.3s cubic-bezier(0.68, -0.55, 0.27, 1.55);
        }
        

        .dropdown-button {
            display: flex;
            
            text-decoration: none;
            font-size: 1rem;
            color: #fff;
            border: none;
            background: none;
            width: 1rem;
            text-align: left;
            cursor: pointer;
            outline: none;
            margin-left: 1rem;
            padding-right:0;
            padding-top: 1rem;
            padding-bottom: 1rem;
        }
        .dropdown-button:hover {
            color: #e2c3ff;
        }

    input[type="checkbox"] {
    /* Add if not using autoprefixer */
    -webkit-appearance: none;
    appearance: none;
    /* For iOS < 15 to remove gradient background */
    background-color: #fff;
    /* Not removed via appearance */
    margin: 0;
    }

    input[type="checkbox"] {
        appearance: none;
        background-color: #fff;
        margin: 0;
        font: inherit;
        color: currentColor;
        width: 1.15em;
        height: 1.15em;
        border: 0.15em solid currentColor;
        border-radius: 0.15em;
        transform: translateY(-0.075em);

        display: grid;
        place-content: center;
    }

    input[type="checkbox"]::before {
        content: "";
        width: 0.65em;
        height: 0.65em;
        transform: scale(0);
        transition: 120ms transform ease-in-out;
        box-shadow: inset 1em 1em darkorchid;
        background-color: CanvasText;

        transform-origin: bottom left;
        clip-path: polygon(14% 44%, 0 65%, 50% 100%, 100% 16%, 80% 0%, 43% 62%);
    }

    input[type="checkbox"]:checked::before {
        transform: scale(1);
    }

</style>