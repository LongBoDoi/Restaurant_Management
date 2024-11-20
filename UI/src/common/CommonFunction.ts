import moment from "moment-timezone";
import EventBus from "./EventBus";

class CommonFunction {
    /**
     * Hiển thị số giây thành giờ:phút:giây
     * @param seconds 
     * @returns 
     */
    formatTimeBySecond = (seconds:number) => {
        const hours = Math.floor(seconds / 3600);
        const minutes = Math.floor((seconds % 3600) / 60);
        const secs = seconds % 60;

        const formattedHours = hours > 0 ? String(hours).padStart(2, '0') : '';
        const formattedMinutes = minutes > 0 ? String(minutes).padStart(2, '0') : '';
        const formattedSeconds = secs > 0 ? String(secs).padStart(2, '0') : '';

        return `${formattedHours}:${formattedMinutes}:${formattedSeconds}`;
    };

    /**
     * Định dạng ngày giờ
     */
    formatDateTime = (date:Date) => {
        const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
        return moment((moment(date).tz(timeZone) as any)._d).format('DD/MM/YYYY HH:mm:ss');
    };

    /**
     * Định dạng giờ
     */
    formatTime = (date:Date) => {
        const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
        return moment((moment(date).tz(timeZone) as any)._d).format('HH:mm');
    };

    /**
     * Định dạng ngày
     */
    formatDate = (date:Date) => {
        return moment(date).format('DD/MM/YYYY');
    };

    showDialog = (dialogData:any) => {
        EventBus.emit('ShowDialog', dialogData);
    }
};

export default CommonFunction;