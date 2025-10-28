﻿"use client";
import { Button } from "@heroui/button";
import Link from "next/link";
import { Tab, Tabs } from "@heroui/tabs";

type Props = {
    tag?:string
    total:number
}


export default function QuestionHeader ({tag,total}:Props) {
    
    const tabs = [
        {key:'newest', label:'Newest'},
        {key:'active', label:'Active'},
        {key:'unanswered', label:'Unanswered'},
    ]
    
    return (
        <div className='flex flex-col border-b w-full gap-4 pb-4'>
            <div className="flex justify-between px-6">
                <div className="text-3xl font-semibold">
                    {tag ? `[${tag}]`:'Newest Questions'}
                </div>
                <Button as={Link} href='/question/ask' color='secondary'>
                    Ask Question
                </Button>
            </div>
            <div className="flex justify-between items-center px-6">
                <div>{total} {total === 1 ? 'Question' : "Questions"}</div>
                <div className="flex items-center">
                    <Tabs>
                        {tabs.map(tab => (
                            <Tab key={tab.key} title={tab.label}/>
                        ))}
                    </Tabs>
                </div>
            </div>
        </div>
    );
}