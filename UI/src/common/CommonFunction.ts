import moment from "moment-timezone";
import EventBus from "./EventBus";
import EventName from "./EventName";

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
        const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
        return moment((moment(date).tz(timeZone) as any)._d).format('HH:mm');
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
     * Xử lý exception khi gọi service
     */
    static handleException = (e:any) => {
        EventBus.emit(EventName.ShowToastMessage, {
            Message: 'Lỗi hệ thống.',
            Type: 'error'
        });
    };
};

export default CommonFunction;