declare module 'vue-the-mask' {
  import { Directive } from 'vue';

  // Export the main plugin type
  const VueTheMask: {
    install: (app: any) => void;
  };

  // Export the directive itself if needed
  export const mask: Directive;

  export default VueTheMask;
}