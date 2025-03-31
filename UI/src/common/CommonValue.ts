class CommonValue {
    static GuidEmpty = '00000000-0000-0000-0000-000000000000';

    static moneyConfig = {
        decimal: ',',
        thousands: '.',
        precision: 0
    };

    static quantityConfig = {
        decimal: ',',
        thousands: '.',
        precision: 3,
        masked: false
    };

    static textFieldRequireRule = (value: string|undefined):boolean => {
        return value !== undefined && value !== '';
    }
}

export default CommonValue;