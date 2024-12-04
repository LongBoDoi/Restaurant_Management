declare module 'v-money' {
    import { Directive } from 'vue';
  
    // Export the main plugin type
    const money: {
      install: (app: any) => void;
    };
  
    // Export the directive itself if needed
    export const VMoney: Directive;
  
    export default VueTheMask;
  }