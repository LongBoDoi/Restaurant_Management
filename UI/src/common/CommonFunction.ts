import moment from "moment-timezone";
import EventBus from "./EventBus";
import EventName from "./EventName";
import Config from "./Config";
import { session } from "./Session";

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

    static showDialog = (dialogData:any) => {
        EventBus.emit(EventName.ShowDialog, dialogData);
    };

    /**
     * Lấy đường dẫn file ảnh trên BE
     */
    static getImageUrl = (fileName: string) => {
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
    static formatThousands = (number: number) => {
        return number?.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".") ?? '';
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