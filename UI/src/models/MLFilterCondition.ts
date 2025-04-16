interface MLFilterCondition {
    Name: string,
    Operator: '=='|'EqualDate'|'>='|'<='|'<',
    Value: any
}

export default MLFilterCondition;