import { EnumExerciseCategory } from '@/common/Enumeration';
import { Exercise, MLActionResult, Test } from '@/models'
import { attemptService } from '@/services/attemptService';
import { testService } from '@/services/testService';
import { defineStore } from 'pinia'

export const testStore = defineStore('testStore', {
  state: () => {
    return {
      _testData: <Test|undefined>undefined,
      _listeningExercises: <Exercise[]>[],
      _grammarExercises: <Exercise[]>[]
    }
  },

  getters: {
    testData(state) {
      return state._testData;
    },

    listeningExercises(state) {
      return state._listeningExercises;
    },

    grammarExercises(state) {
      return state._grammarExercises;
    }
  },

  actions: {
    resetTestData() {
      this._testData = undefined;
      this._listeningExercises = [];
      this._grammarExercises = [];
    },

    /**
     * Lấy dữ liệu bài test
     */
    async getTestDetail(testID:string, userID?:string) {
      try {
        if (testID) {
          const test:Test|undefined = await testService.getTestDetail(testID, userID);
          if (test !== undefined) {
            this._testData = test;
            this._listeningExercises = test.Exercises.filter(e => e.ExerciseCategory === EnumExerciseCategory.Listening);
            this._grammarExercises = test.Exercises.filter(e => e.ExerciseCategory === EnumExerciseCategory.Grammar);
          }
        }
      } catch {
      }
    },

    /**
     * Bắt đầu làm bài
     */
    async makeAnAttempt(testID:string) {
      const result:MLActionResult|undefined = await attemptService.makeAnAttempt(testID);
      if (result?.Success && this._testData) {
        this._testData.UserAttempt = result.Data;
      }
    }
  }
})
