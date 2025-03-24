/**
 * plugins/vuetify.ts
 *
 * Framework documentation: https://vuetifyjs.com`
 */

// Styles
import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'
import '/src/styles/main.scss';

// Composables
import { createVuetify } from 'vuetify'
import { VTimePicker } from 'vuetify/labs/VTimePicker'

// https://vuetifyjs.com/en/introduction/why-vuetify/#feature-guides
export default createVuetify({
  theme: {
    defaultTheme: 'green',
    themes: {
      green: {
        colors: {
          primary: 'rgb(16, 185, 129)',
          secondary: 'rgb(245, 158, 11)',
          // surface: '#F9FAFB'
        }
      }
    }
  },

  components: {
    VTimePicker
  }
})
