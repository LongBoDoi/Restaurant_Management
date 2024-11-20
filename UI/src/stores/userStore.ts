import { defineStore } from 'pinia'

export const userStore = defineStore('users', {
  state: () => ({
    _userID: ''
  }),

  getters: {
    userID(state) {
      return state._userID;
    }
  },

  // #DO NOT USE ARROW FUNCTIONS
  actions: {
    setUserID(userID:string) {
      (this as any)._userID = userID;
    },
  }
})
