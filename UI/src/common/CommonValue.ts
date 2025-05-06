class CommonValue {
    static GuidEmpty = '00000000-0000-0000-0000-000000000000';

    static NewGuid = ():string => {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (char) => {
            const random = (crypto.getRandomValues(new Uint8Array(1))[0] % 16) | 0;
            const value = char === 'x' ? random : (random & 0x3) | 0x8;
            return value.toString(16);
        });
    }

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
    };

    static textEmailRule = (value: string):boolean => {
        return !value || /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
    }

    static textNoSpaceRule = (value: string):boolean => {
        return !value.includes(' ');
    };

    static textPasswordRule = (value: string):boolean => {
        return /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$/.test(value);
    };

    static positiveNumberRule = (value: number):boolean => {
        return value >= 0;
    }
}

export default CommonValue;