// src/types/vue.d.ts
import { ComponentCustomProperties } from 'vue';
import { Router, RouteLocationNormalizedLoaded } from 'vue-router';
import SessionInterface from '@/common/Session';
import ServiceInterface from '@/services';
import EnumerationInterface from '@/common/Enumeration';
import CommonFunction from '@/common/CommonFunction';

declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    $router: Router,
    $route: RouteLocationNormalizedLoaded,
    $session: SessionInterface,
    $service: ServiceInterface,
    $enumeration: EnumerationInterface,
    $commonFunction: CommonFunction
  }
}