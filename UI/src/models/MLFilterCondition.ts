interface MLFilterCondition {
    Name: string,
    Operator: '=='|'EqualDate'|'>='|'<='|'<'|'IN'|'>',
    Value: any,
    CompareProperties: boolean
}

export default MLFilterCondition;