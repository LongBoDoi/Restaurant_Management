// src/types/vue.d.ts
import { ComponentCustomProperties } from 'vue';
import { Router, RouteLocationNormalizedLoaded } from 'vue-router';
import SessionInterface from '@/common/Session';
import ServiceInterface from '@/services';
import EnumerationInterface from '@/common/Enumeration';
import CommonFunction from '@/common/CommonFunction';
import LocalStorageKey from '@/common/LocalStorageKey';
import EventName from '@/common/EventName';
import Config from '@/common/Config';
import { Setting } from '@/models';
import CommonValue from '@/common/CommonValue';
import moment from 'moment';

declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    $router: Router,
    $route: RouteLocationNormalizedLoaded,
    $session: SessionInterface,
    $service: ServiceInterface,
    $enumeration: EnumerationInterface,
    $commonFunction: typeof CommonFunction,
    $localStorageKey: typeof LocalStorageKey,
    $eventName: typeof EventName,
    $config: typeof Config,
    $commonValue: typeof CommonValue,
    $moment: typeof moment
  }
}