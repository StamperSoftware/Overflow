'use server';

import { Question } from "@/lib/types";

export async function getQuestions(tag?:string) : Promise<Question[]>{
    let url = 'http://localhost:8001/questions';
    if (tag) url += `?tag=${tag}`
    const response = await fetch(url);
    
    if (!response.ok) throw new Error('failed to get data');
    
    return response.json();
}

export async function getQuestion(id:string) : Promise<Question>{
    const url = `http://localhost:8001/questions/${id}`;
    const response = await fetch(url);
    if (!response.ok) throw new Error('failed to get data');
    return response.json();
}