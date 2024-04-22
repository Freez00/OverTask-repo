import { persisted } from "svelte-persisted-store";
import type {Task,Category} from "$lib/scripts/todoHandler";

interface records {
    Category: Category;
    Tasks: Task[];
}

export const localTaskInformation = persisted("localTaskInformation", {
    records: [{
        Category: {
            Id: -1,
            Title: "Без категория",
            Color: "#CCCCCC"
        },
        Tasks: [] as Task[]
    }] as records[]
});

export const todoLocalIndex = persisted("todoLocalIndex", 0);
export const categoryLocalIndex = persisted("categoryLocalIndex", 0);