import { getToken } from "./requestHandler";
import { backendURL } from "./variables";

export interface Group{
    Id:number,
    Name:string,
    JoinCode:string,
    Users?:GroupUser[],
    OwnerID:number,
    PictureURL:string
}

export interface GroupUser{
    Id:number,
    Name:string,
    PictureURL:string
}

export async function createGroup(name:string, joinCode:string){
    const resource = {
        Name:name,
        JoinCode:joinCode
    };
    
    const response = await fetch(`${backendURL}/group/create`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${await getToken()}`,
          'Content-Type': 'application/json'
        },
        body:JSON.stringify(resource)
    });
    if(response.ok){

        let data = await response.json();
        console.log(data)
        const group:Group = {
            Id: data.group.id,
            Name: data.group.name,
            JoinCode: data.group.joinCode,
            OwnerID: data.group.ownerID,
            PictureURL: data.group.pictureURL,
            Users: data.users.map((user:any) => ({
                Id: user.id,
                Name: user.userName,
                PictureURL: user.profilePictureURL
            }))
        };
        return group;
    }
    return undefined;
}

export async function joinGroup(joinCode:string){
    const response = await fetch(`${backendURL}/group/join`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${await getToken()}`,
          'Content-Type': 'application/json'
        },
        body:JSON.stringify(joinCode)
    });
    if(response.ok){
        let data = await response.json();
        const group:Group = {
            Id: data.group.id,
            Name: data.group.name,
            JoinCode: data.group.joinCode,
            OwnerID: data.group.ownerID,
            PictureURL: data.group.pictureURL,
            Users: data.users.map((user:any) => ({
                Id: user.id,
                Name: user.userName,
                PictureURL: user.profilePictureURL
            }))
        };
        return group;
    }
    return undefined;
}

export async function promoteUser(groupID:number, userID:number){
    const resource:Number[] = [groupID, userID];
    const response = await fetch(`${backendURL}/group/promote`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${await getToken()}`,
          'Content-Type': 'application/json'
        },
        body:JSON.stringify(resource)
    });
    if(response.ok){
        let data = await response.json();
        console.log("data is");
        console.log(data);
        const group:Group = {
            Id: data.group.id,
            Name: data.group.name,
            JoinCode: data.group.joinCode,
            OwnerID: data.group.ownerID,
            PictureURL: data.group.pictureURL,
            Users: data.users.map((user:any) => ({
                Id: user.id,
                Name: user.userName,
                PictureURL: user.profilePictureURL
            }))
        };
        return group;
    }
    return undefined;
}

export async function kickUser(groupID:number, userID:number){
    const resource:Number[] = [groupID, userID];
    const response = await fetch(`${backendURL}/group/kick`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${await getToken()}`,
          'Content-Type': 'application/json'
        },
        body:JSON.stringify(resource)
    });
    if(response.ok){
        let data = await response.json();
        const group:Group = {
            Id: data.group.id,
            Name: data.group.name,
            JoinCode: data.group.joinCode,
            OwnerID: data.group.ownerID,
            PictureURL: data.group.pictureURL,
            Users: data.users.map((user:any) => ({
                Id: user.id,
                Name: user.userName,
                PictureURL: user.profilePictureURL
            }))
        };
        return group;
    }
    return undefined;

}

export async function editGroup(groupInfo:Group){
    const resource = {
        Id:groupInfo.Id,
        Name:groupInfo.Name,
        PictureURL:groupInfo.PictureURL
    }
    const response = await fetch(`${backendURL}/group/edit`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${await getToken()}`,
          'Content-Type': 'application/json'
        },
        body:JSON.stringify(resource)
    });
    if(response.ok){
        let data = await response.json();
        const group:Group = {
            Id: data.group.id,
            Name: data.group.name,
            JoinCode: data.group.joinCode,
            OwnerID: data.group.ownerID,
            PictureURL: data.group.pictureURL,
            Users: data.users.map((user:any) => ({
                Id: user.id,
                Name: user.userName,
                PictureURL: user.profilePictureURL
            }))
        };
        return group;
    }
    return undefined;
}

export async function leaveGroup(groupID:number) {
    const response = await fetch(`${backendURL}/group/leave`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${await getToken()}`,
          'Content-Type': 'application/json'
        },
        body:JSON.stringify(groupID)
    });
}

export async function deleteGroup(groupID:number) {
    const response = await fetch(`${backendURL}/group/delete`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${await getToken()}`,
          'Content-Type': 'application/json'
        },
        body:JSON.stringify(groupID)
    });
}

export async function getGroups(){
    const response = await fetch(`${backendURL}/account/getGroups`,{
        method:'GET',
        headers:{
            'Authorization': `Bearer ${await getToken()}`,
            'Content-Type': 'application/json'
        }
    });
    var data = await response.json();
    console.log(data);
    if(data){
        console.log(data);
        const groups:Group[] = await data.map((record:any) => ({
            Id: record.group.id,
            Name: record.group.name,
            JoinCode: record.group.joinCode,
            OwnerID: record.group.ownerID,
            PictureURL: record.group.pictureURL,
            Users: record.users.map((user:any) => ({
                Id: user.id,
                Name: user.userName,
                PictureURL: user.profilePictureURL
            }))
        }));
        return groups;
    }
    return [];
}