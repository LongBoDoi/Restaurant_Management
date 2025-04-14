import moment from "moment-timezone";
import EventBus from "./EventBus";
import EventName from "./EventName";
import Config from "./Config";
import { session } from "./Session";
import { useDisplay } from "vuetify";

class CommonFunction {
    /**
     * Hiển thị số giây thành giờ:phút:giây
     * @param seconds 
     * @returns 
     */
    static formatTimeBySecond = (seconds:number) => {
        const hours = Math.floor(seconds / 3600);
        const minutes = Math.floor((seconds % 3600) / 60);
        // const secs = seconds % 60;

        const formattedHours = hours > 0 ? String(hours).padStart(2, '0') : '';
        const formattedMinutes = minutes > 0 ? String(minutes).padStart(2, '0') : '';
        // const formattedSeconds = secs > 0 ? String(secs).padStart(2, '0') : '';

        return `${formattedHours ? `${formattedHours}g` : ''}${formattedMinutes}p`;
    };

    /**
     * Định dạng ngày giờ
     */
    static formatDateTime = (date:Date) => {
        return moment.utc(date).local().format('DD/MM/YYYY HH:mm');
    };

    /**
     * Định dạng giờ
     */
    static formatTime = (date:Date, noTimeZone?:boolean) => {
        if (noTimeZone) {
            return moment(date).format('HH:mm');
        }

        return moment.utc(date).local().format('HH:mm');
    };

    /**
     * Định dạng ngày
     */
    static formatDate = (date:Date) => {
        return moment(date).format('DD/MM/YYYY');
    };

    static getDateValueFormat = (date:Date) => {
        return moment(date).format('YYYY-MM-DD');
    };

    /**
     * Lấy giờ UTC
     */
    static getUTCDate = (date: Date) => {
        const utcDate = moment(date).utc().format();
        return utcDate;
    };

    static showDialog = (dialogData:any) => {
        EventBus.emit(EventName.ShowDialog, dialogData);
    };

    /**
     * Lấy đường dẫn file ảnh trên BE
     */
    static getImageUrl = (fileName: string|undefined) => {
        if (!fileName) {
            return '';
        }
        return `${import.meta.env.VITE_API_URL}/uploads/${fileName}`;
    }

    static showToastMessage = (message:string, type:'error'|'success') => {
        EventBus.emit(EventName.ShowToastMessage, {
            Message: message,
            Type: type
        });
    };

    /**
     * Lấy giá trị thiết lập
     */
    static getSettingValue = (settingKey: string):any => {
        return session.Settings?.find(s => s.SettingKey === settingKey)?.Value;
    };

    /**
     * Định dạng số theo hàng nghìn
     */
    static formatThousands = (number: number|undefined) => {
        if (number === undefined) {
            return '';
        }
        // return number?.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".") ?? '';
        return number?.toLocaleString('de-DE', { minimumFractionDigits: 0, maximumFractionDigits: 3 }) ?? '';
    };

    /**
     * Định dạng số điện thoại
     */
    static formatPhoneNumber = (phoneNumber: string|undefined) => {
        if (!phoneNumber) return "";
        return phoneNumber.replace(/(\d{4})(\d{3})(\d{3})/, "$1 $2 $3");
    };

    /**
     * Lấy giá trị SĐT
     */
    static getRealPhoneNumberValue = (phoneNumber: string|undefined) => {
        if (!phoneNumber) return '';
        return phoneNumber.replace(/\D/g, "");
    };

    /**
     * Trả về dữ liệu số thực từ số bị format theo dạng ###.###,###
     */
    static getRealFloatValue = (number: number|undefined) => {
        return parseFloat(number?.toString().replaceAll('.', '').replaceAll(',', '.') ?? '0');
    };

    /**
     * Lấy code kích thước màn hình
     * xs
     * sm
     * md
     * lg
     * xl
     */
    static getScreenCode = ():string => {
        const { name } = useDisplay();
        return name.value;
    }

    /**
     * Xử lý exception khi gọi service
     */
    static handleException = (e:any) => {
        var errorDetail:string = '';
        if(Config.ShowErrorOnToast) {
            errorDetail = ` ${e.message}`;
        }

        EventBus.emit(EventName.ShowToastMessage, {
            Message: `Lỗi hệ thống.${errorDetail}`,
            Type: 'error'
        });
    };
};

export default CommonFunction;