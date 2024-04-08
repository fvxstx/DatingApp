import { ActivatedRouteSnapshot, ResolveFn } from '@angular/router';
import { Member } from '../_models/member';
import { inject } from '@angular/core';
import { MembersService } from '../_services/members.service';

export const memberDetailedResolver: ResolveFn<Member> = (
  route: ActivatedRouteSnapshot,
  state
) => {
  return inject(MembersService).getMember(route.paramMap.get('username')!);
};
