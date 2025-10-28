﻿import { getQuestion } from "@/lib/actions/question-actions";
import { notFound } from "next/navigation";
import QuestionDetailsHeader from "@/app/questions/[id]/QuestionDetailsHeader";
import QuestionContent from "@/app/questions/[id]/QuestionContent";
import AnswerContent from "@/app/questions/[id]/AnswerContent";
import AnswerHeader from "@/app/questions/[id]/AnswerHeader";

type Params = Promise<{id:string}>

export default async function QuestionDetailPage ({params}: { params: Params }) {
    const {id} = await params;
    const question = await getQuestion(id);

    if (!question) return notFound();

    return (
        <div className={'w-full'}>
            <QuestionDetailsHeader question={question}/>
            <QuestionContent question={question}/>
            {question.answers.length && <AnswerHeader answerCount={question.answers.length}/>}
            {question.answers.map(answer => <AnswerContent key={answer.id} answer={answer}/>)}
        </div>
    );
}