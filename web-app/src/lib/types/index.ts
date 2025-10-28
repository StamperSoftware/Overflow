export interface Question {
    id: string
    title: string
    content: string
    askerId: string
    askerDisplayName: string
    createdAt: string
    updatedAt?: string
    viewCount: number
    tagSlugs: string[]
    hasAcceptedAnswer: boolean
    votes: number
    answerCount: number
    answers: Answer[]
}

export interface Answer {
    id: string
    content: string
    userId: string
    userDisplayName: string
    createdAt: string
    updatedAt?: string
    accepted: boolean
    questionId: string
}