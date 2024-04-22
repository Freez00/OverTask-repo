import { getToken } from "./requestHandler";
import { backendURL } from "./variables";
import { browser } from '$app/environment';
import { get } from "svelte/store";
import { localTaskInformation, todoLocalIndex, categoryLocalIndex } from "./localTasksPrst";


export interface Category{
    Id:number,
    Title:string,
    Color:string
}

export interface Task{
    Id:number,
    Title:string,
    Completed:boolean
}


export function getTasksLocal(){
    let taskInfo = get(localTaskInformation)
    return taskInfo.records;
}

export function createTaskLocal(categoryId:number, title:string){
    let taskInfo = getTasksLocal();
    let index = get(todoLocalIndex);
    let task:Task = {
        Id: index++,
        Title: title,
        Completed: false
    };

    for(let record of taskInfo){
        if(record.Category.Id === categoryId){
            record.Tasks.push(task);
            localTaskInformation.set({records: taskInfo});
            todoLocalIndex.set(index);
            return task;
        }
    }
    
    return false;
}

export function deleteTaskLocal(taskId: number) {
    let taskInfo = getTasksLocal();

    for (let record of taskInfo) {
        const taskIndex = record.Tasks.findIndex(task => task.Id === taskId);

        if (taskIndex !== -1) {
            record.Tasks.splice(taskIndex, 1);
        }
    }

    localTaskInformation.set({ records: taskInfo });
}

export function updateTaskLocal(taskId:number, title:string, completed:boolean){
    let taskInfo = getTasksLocal();

    for (let record of taskInfo) {
        const task = record.Tasks.find(task => task.Id === taskId);
        if (task) {
            task.Title = title;
            task.Completed = completed;
        }
    }

    localTaskInformation.set({records: taskInfo});
}

export function createCategoryLocal(title:string, color:string){
    let taskInfo = getTasksLocal();
    let index = get(categoryLocalIndex);
    let category = {
        Id: index,
        Title: title,
        Color: color,
    };

    taskInfo.push({ Category: category, Tasks: [] });

    localTaskInformation.set({records: taskInfo});
    categoryLocalIndex.set(index + 1);

}

export function updateCategoryLocal(categoryId:number, title:string, color:string){
    let taskInfo = getTasksLocal();

    for (let record of taskInfo) {
        if (record.Category.Id === categoryId) {
            record.Category.Title = title;
            record.Category.Color = color;
        }
    }

    localTaskInformation.set({records: taskInfo});
}

export function deleteCategoryLocal(categoryId: number) {
    console.log("called delete on")
    console.log(categoryId)
    let taskInfo = getTasksLocal();
    
    taskInfo = taskInfo.filter(record => record.Category.Id !== categoryId);

    localTaskInformation.set({records: taskInfo});
}

export async function getTasksAPI(){
    const response = await fetch(`${backendURL}/todo/get`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${await getToken()}`
        }
    });
    if(response.ok){
        let data = await response.json();
        let info = data.map((record:any) => ({
            Category: {
                Id: record.category.id, 
                Title: record.category.title,
                Color: record.category.color
            } as Category,
            Tasks: record.tasks.map((task:any) => ({
                Id: task.id,
                Title: task.title,
                Completed: task.completed
            })) as Task[]
        }));

        return info;
    }
    return undefined;

}

export async function createTaskAPI(categoryId:number, title:string){
    const resource = {
        CategoryId: categoryId,
        Title: title,
        Completed: false
    }
    const response = await fetch(`${backendURL}/todo/create`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${await getToken()}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(resource)
    });

    if(response.ok){
        var data = await response.json();
        var task:Task = {
            Id: data.id,
            Title: data.title,
            Completed: data.completed
        }
        return task;
    }
    return undefined;
}

export async function deleteTaskAPI(taskId:number){
    const response = await fetch(`${backendURL}/todo/delete`, {
        method: 'DELETE',
        headers: {
            'Authorization': `Bearer ${await getToken()}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(taskId)
    });
    if(response.ok){
        return true;
    }
    return false;
}

export async function updateTaskAPI(taskId:number, title:string, completed:boolean){
    const resource = {
        Id: taskId,
        Title: title,
        Completed: completed
    }

    const response = await fetch(`${backendURL}/todo/update`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${await getToken()}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(resource)
    });
    if(response.ok){
        return true;
    }
    
    return false;
}


export async function createCategoryAPI(title:string, color:string){
    const resource ={
        Title:title,
        Color:color,
        CategoryID: -1
    }
    
    const response = await fetch(`${backendURL}/todo/category/create`, {
        method:'POST',
        headers: {
            'Authorization': `Bearer ${await getToken()}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(resource)
    });

    if(response.ok){
        return true;
    }

    return false;
}

export async function updateCategoryAPI(categoryId:number, title:string, color:string){
    const resource = {
        CategoryID: categoryId,
        Title: title,
        Color: color
    };
    console.log(resource);
    const response = await fetch(`${backendURL}/todo/category/rename`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${await getToken()}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(resource)
    });
    console.log(response);
    if(response.ok){
        return true;
    }

    return false;
}

export async function deleteCategoryAPI(categoryId:number){

    const response = await fetch(`${backendURL}/todo/category/delete`, {
        method: 'DELETE',
        headers: {
            'Authorization': `Bearer ${await getToken()}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(categoryId)
    });
    if(response.ok){
        return true;
    }

    return false;
}

export async function getCategoriesAPI(){
    const response = await fetch(`${backendURL}/todo/category/get`,{
        method:'GET',
        headers: {
            'Authorization': `Bearer ${await getToken()}`,
            'Content-Type': 'application/json'
        }
    });
    
    if(response.ok){
        const data = await response.json()
        let categories = data.map((record:any) => ({
            Id: record.id,
            Title: record.title,
            Color: record.color
        })) as Category[];
        categories = [
            {
                Id: -1,
                Title: "None",
                Color: "#CCCCCC"
            },
            ...categories
        ];

        return categories;
    }
    
    return [];
}
