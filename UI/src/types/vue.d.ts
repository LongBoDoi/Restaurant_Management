// src/types/vue.d.ts
import { ComponentCustomProperties } from 'vue';
import { Router, RouteLocationNormalizedLoaded } from 'vue-router';
import SessionInterface from '@/common/Session';
import ServiceInterface from '@/services';
import EnumerationInterface from '@/common/Enumeration';
import CommonFunction from '@/common/CommonFunction';
import LocalStorageKey from '@/common/LocalStorageKey';
import EventName from '@/common/EventName';

declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    $router: Router,
    $route: RouteLocationNormalizedLoaded,
    $session: SessionInterface,
    $service: ServiceInterface,
    $enumeration: EnumerationInterface,
    $commonFunction: typeof CommonFunction,
    $localStorageKey: LocalStorageKey,
    $eventName: typeof EventName
  }
}