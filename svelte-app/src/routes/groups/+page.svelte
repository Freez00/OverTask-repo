<script lang="ts">
    import {createGroup} from '$lib/scripts/groupHandler'
    import {joinGroup} from '$lib/scripts/groupHandler'
    import type {Group} from '$lib/scripts/groupHandler'
    import {getGroups} from '$lib/scripts/groupHandler'
    import {deleteGroup} from '$lib/scripts/groupHandler'
    import {leaveGroup} from '$lib/scripts/groupHandler'
    import {editGroup} from '$lib/scripts/groupHandler'
    import {promoteUser} from '$lib/scripts/groupHandler'
    import {kickUser} from '$lib/scripts/groupHandler'
    import {onMount} from 'svelte'
    import {getUserId} from '$lib/scripts/requestHandler'
    import Navbar from '../navbar.svelte';
    import {goto} from '$app/navigation';
    import {isAuthenticated} from '$lib/scripts/requestHandler';
    import Modal from './Modal.svelte';
    import ManageModal from './ManageModal.svelte';
    import { fly } from 'svelte/transition';
    import { flip } from 'svelte/animate';



    let groups:Group[] = [];
    let userID:number;
    let showCreateModal:Boolean;
    let showJoinModal:Boolean;

    let createGroupName:string;
    let createJoinCode:string = '';

    let joinJoinCode:string;
    let activeGroup:number = -1;
    let activeMembersModal:number = -1;
    let promotedUserId:number = -1;
    let kickedUserId:number = -1;


    onMount(async ()=>{
        if(!(await isAuthenticated())){
            await goto('/');
        }
        userID = await getUserId();
        groups = await getGroups();
    })

    async function handleEditGroup(group:Group){
        activeGroup = -1;

        let newGroup = await editGroup(group);
        if (newGroup){
            console.log("edit group success");
            groups = [];
            groups = await getGroups();
            return true;
        }

    }

    async function handleManageGroup(groupID:number) {
        activeGroup = -1;
        activeGroup = groupID;

  }
    async function handleViewMembers(groupID:number) {
        activeMembersModal = -1;
        activeMembersModal = groupID;
    }

    async function handleCreateGroup(){
        let newGroup = await createGroup(createGroupName, createJoinCode);
        activeGroup=-1;
        if (newGroup){
            groups = await getGroups();
            createGroupName = '';
            createJoinCode = '';
            return true;
        }

    }

    async function handleJoinGroup(){
        let newGroup = await joinGroup(joinJoinCode);
        activeGroup=-1;
        if (newGroup){
            groups = await getGroups();
            joinJoinCode = '';
            return true;
        }

    }

    async function handlePromoteUser(groupID:number, userID:number){
        let newGroup = await promoteUser(groupID, userID);
        if (newGroup){
            promotedUserId = userID;
            setTimeout(() => {
                promotedUserId = -1;
            }, 1000);
            setTimeout(() => {
                groups = groups.map(group => group.Id === groupID ? newGroup : group) as Group[];
            }, 500);
            
            return true;
        }
    }

    async function handleKickUser(groupID:number, userID:number){
        let newGroup = await kickUser(groupID, userID);
        if (newGroup){
            kickedUserId = userID;
            groups = groups.map(group => group.Id === groupID ? newGroup : group) as Group[];
            setTimeout(() => {
                kickedUserId = -1;
                //groups = groups.map(group => group.Id === groupID ? newGroup : group) as Group[];
            }, 1000);
            return true;
        }
    }

    async function handleLeaveGroup(groupID:number){
        await leaveGroup(groupID);
        activeGroup=-1;
        groups = groups.filter(group => group.Id !== groupID)
    }

    async function handleDeleteGroup(groupID:number){
        await deleteGroup(groupID);
        activeGroup=-1;
        groups = groups.filter(group => group.Id !== groupID)
    }

    async function handleCalendarView(groupID:number){
        window.location.replace("/groups/" + groupID);
    }

</script>

    <Navbar/>
        <hr>
        <h1 class="header">Вашите групи</h1>
        <hr>

    <div class="button-holder">
        <button class="create-group-button" on:click={() => (showCreateModal = true)}>
            Създайте група
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor" class="bi bi-patch-plus" viewBox="0 0 16 16">
                <path fill-rule="evenodd" d="M8 5.5a.5.5 0 0 1 .5.5v1.5H10a.5.5 0 0 1 0 1H8.5V10a.5.5 0 0 1-1 0V8.5H6a.5.5 0 0 1 0-1h1.5V6a.5.5 0 0 1 .5-.5"/>
                <path d="m10.273 2.513-.921-.944.715-.698.622.637.89-.011a2.89 2.89 0 0 1 2.924 2.924l-.01.89.636.622a2.89 2.89 0 0 1 0 4.134l-.637.622.011.89a2.89 2.89 0 0 1-2.924 2.924l-.89-.01-.622.636a2.89 2.89 0 0 1-4.134 0l-.622-.637-.89.011a2.89 2.89 0 0 1-2.924-2.924l.01-.89-.636-.622a2.89 2.89 0 0 1 0-4.134l.637-.622-.011-.89a2.89 2.89 0 0 1 2.924-2.924l.89.01.622-.636a2.89 2.89 0 0 1 4.134 0l-.715.698a1.89 1.89 0 0 0-2.704 0l-.92.944-1.32-.016a1.89 1.89 0 0 0-1.911 1.912l.016 1.318-.944.921a1.89 1.89 0 0 0 0 2.704l.944.92-.016 1.32a1.89 1.89 0 0 0 1.912 1.911l1.318-.016.921.944a1.89 1.89 0 0 0 2.704 0l.92-.944 1.32.016a1.89 1.89 0 0 0 1.911-1.912l-.016-1.318.944-.921a1.89 1.89 0 0 0 0-2.704l-.944-.92.016-1.32a1.89 1.89 0 0 0-1.912-1.911z"/>
              </svg>
        </button>
        <button class="join-group-button" on:click={() => (showJoinModal = true)}>
            Присъединяване
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor" class="bi bi-box-arrow-in-right" viewBox="0 0 16 16">
                <path fill-rule="evenodd" d="M6 3.5a.5.5 0 0 1 .5-.5h8a.5.5 0 0 1 .5.5v9a.5.5 0 0 1-.5.5h-8a.5.5 0 0 1-.5-.5v-2a.5.5 0 0 0-1 0v2A1.5 1.5 0 0 0 6.5 14h8a1.5 1.5 0 0 0 1.5-1.5v-9A1.5 1.5 0 0 0 14.5 2h-8A1.5 1.5 0 0 0 5 3.5v2a.5.5 0 0 0 1 0z"/>
                <path fill-rule="evenodd" d="M11.854 8.354a.5.5 0 0 0 0-.708l-3-3a.5.5 0 1 0-.708.708L10.293 7.5H1.5a.5.5 0 0 0 0 1h8.793l-2.147 2.146a.5.5 0 0 0 .708.708z"/>
              </svg>
        </button>
    </div>

    <div class="group-container">
        {#each groups as group, index}
        <div class="group" style="animation-delay: {index * 0.07}s; visibility:hidden;">
            <button class="view-members" on:click={async() => {await handleViewMembers(group.Id); activeMembersModal=-1;}}>
                <i class="fa-solid fa-user-group"></i>
            </button>
            <ManageModal showModal = {group.Id === activeMembersModal} 
            buttonAction={async () => {return true;}} buttonTitle="Готово" badMessage="Error occured." 
            deleteButtonAction={undefined}> 
                <p class="group-name">{group.Name}</p>
                <hr>
                <div class="members-list">
                    {#if group.Users}
                        {#each group.Users as user (user.Id)}
                        <div id="user-{user.Id}" class="member-row {user.Id === promotedUserId ? 'promoted' : ''}" 
                            animate:flip={{ duration: 2000 }}>
                            <img class="member-icon" src={user.PictureURL} alt={user.Name} />
                            <div class="member-info">
                                {#if group.OwnerID == user.Id}
                                    <i style="color:orange" class="fas fa-crown"></i>
                                {/if}
                                {user.Name} 
                                {#if user.Id == userID}
                                    <i style="font-size:small">(you)</i>
                                {/if}
                            </div>
                            {#if group.OwnerID == userID && group.OwnerID != user.Id}
                            <div class="member-buttons">
                                <button class="member-button-action promote" on:click={async () => {await handlePromoteUser(group.Id, user.Id)}}>
                                    <i class="fas fa-crown"></i>
                                </button>
                                <button class="member-button-action kick" on:click={async () => {await handleKickUser(group.Id, user.Id)}}>
                                    <i class="fas fa-x"></i>
                                </button>
                            </div>
                            {/if}
                        </div>
                    {/each}
                    {/if}
                </div>
            </ManageModal>
            <button class="go-back" on:click={async () => await handleLeaveGroup(group.Id)}>
                <i class="fas fa-x"></i>
            </button>
            <img class="group-icon" src={group.PictureURL} alt={group.Name} />
            <div class="group-name">{group.Name}</div>
            <div class="group-actions">
                {#if group.OwnerID == userID}
                <button class="manage-button" on:click={async () => {await handleManageGroup(group.Id); activeGroup=-1}}>Настройки</button>
                <ManageModal showModal={group.Id === activeGroup} buttonAction={async () => handleEditGroup(group)} buttonTitle="Запази промените" 
                    badMessage="Couldn't save changes." bind:managedGroup={group} deleteButtonAction={async() => handleDeleteGroup(group.Id)}>
                    <div class="mb-3" >
                        <label for="GroupName" class="form-label">Име на групата <span style="font-size:13px;"></span></label>
                        <input type="text" class="form-control" id="GroupName" bind:value={group.Name} />
                    </div>
                    <div class="mb-3" >
                        <label for="JoinCode" class="form-label">Код за достъп <span style="font-size:13px;"></span></label>
                        <input  type="text" class="form-control" id="JoinCode" bind:value={group.JoinCode} disabled/>
                    </div>
                    <div class="mb-3" >
                        <label for="PictureURL" class="form-label">Снимка</label>
                        <select class="form-select" id="PictureURL-{group.Id}" bind:value={group.PictureURL}>
                            <option value="/group_1.jpg">Група 1</option>
                            <option value="/group_2.jpg">Група 2</option>
                            <option value="/group_3.jpg">Група 3</option>
                            <option value="/group_4.jpg">Група 4</option>
                            <option value="/group_5.jpg">Група 5</option>
                        </select>
                    </div>
                </ManageModal>
                {/if}
                <button class="calendar-button" on:click={async () => await handleCalendarView(group.Id)}>Календар</button>
            </div>
        </div>
        {/each}
    </div>
    
    <Modal bind:showModal={showCreateModal} buttonAction={handleCreateGroup} buttonTitle="Създаване" badMessage="Група с въведения код вече съществува">
        <div class="mb-3" >
            <label for="GroupName" class="form-label">Име на група&nbsp;&nbsp;<span style="font-size:13px;">(задължително)</span></label>
            <input type="text" class="form-control" id="GroupName" bind:value={createGroupName} />
        </div>
        <div class="mb-3" >
            <label for="JoinCode" class="form-label">Код за достъп<span style="font-size:13px;"></span></label>
            <input type="text" class="form-control" id="JoinCode" placeholder="(Автоматично генериран)" bind:value={createJoinCode}/>
        </div>
    </Modal>
    
    <Modal bind:showModal={showJoinModal} buttonAction={handleJoinGroup}  buttonTitle="Присъединяване" badMessage="Групата не е намерена или вече сте вътре">
        <div class="mb-3" >
            <label for="JoinCode" class="form-label">Код за достъп<span style="font-size:13px;"></span></label>
            <input type="text" class="form-control" id="JoinCode"  bind:value={joinJoinCode}/>
        </div>
    </Modal>
    
    <style>

        .button-holder{
            width: 100%;
            display: flex;
            justify-content: space-around;
        }

        .group-container {
            display: flex;
            flex-wrap: wrap;
            justify-content: center;
        }
        
        .group {
            display: flex;
            flex-direction: column;
            align-items: center;
            padding: 10px;
            border: 3px solid #373c4f;
            border-radius: 40px;
            margin: 10px;
            width: 300px;
            min-height: 300px;

            position: relative;
            background: transparent;
            animation: spread 1s ease-out forwards;
        }
        @keyframes spread {
        0% {
            visibility: hidden;
            transform: scale(0);
            opacity: 0;
        }
        100% {
            transform: scale(1);
            opacity: 1;
            visibility: visible;
        }
    }

        .group:hover{
            border: 3px solid ;
            background-color: rgb(207, 191, 218);
        }


        .group-icon {
            width: 150px; /* Increased width */
            height: 150px; /* Increased height */
            margin-bottom: 30px;
            margin-top:15px;
            border-radius: 200px;
            border: 5px solid darkorchid;
            background-color: white;
        }

        .group-name {
            text-align: center;
            font-weight: bold;
            width: 100%;
            margin-bottom: 5px;
            font-size:18px;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        .group-actions {
            display: flex;
            justify-content: space-evenly;
            width: 100%;
        }

        .manage-button {
            background-color: darkorchid;
            color: white;
            padding: 5px 10px;
            border-radius: 3px;
            cursor: pointer;
            margin-top:5px;
            border: 1px solid #373c4f;
        }
        .manage-button:hover, .create-group-button:hover{
            background-color: rgb(181, 115, 212);
        }

        .calendar-button {
            background-color: white;
            color: #333;
            padding: 5px 10px;
            border-radius: 3px;
            cursor: pointer;
            margin-top:5px;
            border: 2px solid #373c4f;
        }
        .calendar-button:hover, .join-group-button:hover{
            background-color: rgb(237, 237, 237);
        }

        .header {
            font-size: 42px;
            font-weight: bold;
            margin-bottom: 20px;
            width: 100%;
            text-align: center;
        }

        .create-group-button {
            background-color: darkorchid;
            color: white;
            padding: 10px 20px;
            border-radius: 5px;
            cursor: pointer;
            margin-bottom: 10px;
        }

        .join-group-button {
            background-color: white;
            color: #333;
            padding: 10px 20px;
            border-radius: 5px;
            cursor: pointer;
            margin-left: 10px;
            margin-bottom: 10px;
        }

        .form-label {
            font-weight: bold;
            color:#6e0092;
        }

        .form-control {
            width: 100%;
            padding: 10px;
            margin-bottom: 10px;
            border: 1px solid #b0abcf;
            border-radius: 7px;
        }
        .go-back{
            position: absolute;
            top: 20px;
            right: 20px;
            width: 30px;
            height: 30px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1rem;
            color: #000000;
            border: 2px solid #000000;
            opacity: 0.5;
            border-radius: 50%;
            background-color: transparent;
            cursor: pointer;
        }
        .go-back:hover {
            opacity: 1;
            color:#E04B5A;
            border-color:#E04B5A;
        }
        .view-members{
            position: absolute;
            top: 20px;
            left: 20px;
            width: 30px;
            height: 30px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1rem;
            color: #000000;
            border: 2px solid #000000;
            opacity: 0.5;
            border-radius: 50%;
            background-color: transparent;
            cursor: pointer;
        }
        .view-members:hover {
            opacity: 1;
        }
   
        .members-list {
            max-height: 300px; /* Adjust this value as needed */
            overflow-y: auto;
            padding-right: 10px;
        }
        
        .member-icon{
            width: 50px;
            height:50px;
            border-radius: 50%;
            border: 3px solid darkorchid;
            background-color: lightgray;
            margin: 0 10px;
        }

        .member-row {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 1em;
            margin-top: 1em;
            position: relative;
            overflow: hidden;
            transition: all 1s ease;
        }

        .member-row::before {
            content: "";
            position: absolute;
            top: 0;
            left: -100%;
            width: 100%;
            height: 100%;
            background: linear-gradient(to right, #FFE761, #FFE761);
            transition: all 1s ease;
        }

        .member-row.promoted::before {
            left: 0;
        }

        .member-info {
            flex-grow: 1;
            font-size:large;
            font-weight: 600;
            margin-right: 20px;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        .member-buttons {
            flex-shrink: 0;
        }

        .member-button-action{
            margin: 0px, 3px;
            border:none;
            color: #202020;
            background-color: transparent;
            border-radius: 30%;
            width: 40px;
            height: 40px;
            justify-content: center;
            align-items: center;
            display:inline-block;
            
            transition: 0.5s ease all;
        }
        
        .member-button-action:hover{
            background-color: #e8e8e8;
        }
        
        .promote:hover{
            color:orange;
        }
        
        .kick:hover{
            color:red;
        }

    </style>