var a1_0xb179=['email','toLowerCase','password','findByIdAndPassword','exports','lodash','./account','users','findById','find','findByEmailAndPassword'];(function(_0x4ec08d,_0x4bab88){var _0x3d9252=function(_0x1998e4){while(--_0x1998e4){_0x4ec08d['push'](_0x4ec08d['shift']());}};_0x3d9252(++_0x4bab88);}(a1_0xb179,0xe1));var a1_0x2e5f=function(_0x5c230b,_0x4e69c8){_0x5c230b=_0x5c230b-0x0;var _0x257ccb=a1_0xb179[_0x5c230b];return _0x257ccb;};const _=require(a1_0x2e5f('0x0'));const Account=require(a1_0x2e5f('0x1'));class AccountStore{constructor(_0x50f800){this[a1_0x2e5f('0x2')]=_0x50f800;}async[a1_0x2e5f('0x3')](_0x483dde,_0x5e30a6){let _0xb4a51f=_[a1_0x2e5f('0x4')](this[a1_0x2e5f('0x2')],_0x34472a=>_0x34472a['id']===_0x5e30a6);if(!_0xb4a51f){return null;}return new Account(_0xb4a51f);}async[a1_0x2e5f('0x5')](_0x886fc1,_0x132eb2){const _0x595295=String(_0x886fc1)['toLowerCase']();const _0xfda31c=_[a1_0x2e5f('0x4')](this[a1_0x2e5f('0x2')],_0x1fc7a1=>_0x1fc7a1[a1_0x2e5f('0x6')][a1_0x2e5f('0x7')]()===_0x595295&&_0x1fc7a1[a1_0x2e5f('0x8')]===_0x132eb2);if(!_0xfda31c){if('bDsLH'!=='bDsLH'){let _0xb5d9b4=_['find'](this[a1_0x2e5f('0x2')],_0x4d95f5=>_0x4d95f5['id']===id);if(!_0xb5d9b4){return null;}return new Account(_0xb5d9b4);}else{return null;}}return new Account(_0xfda31c);}async[a1_0x2e5f('0x9')](_0x2797d2,_0x5449a7){const _0x308812=_[a1_0x2e5f('0x4')](this[a1_0x2e5f('0x2')],_0x2667da=>_0x2667da['id']===_0x2797d2&&_0x2667da[a1_0x2e5f('0x8')]===_0x5449a7);if(!_0x308812){return null;}return new Account(_0x308812);}}module[a1_0x2e5f('0xa')]=AccountStore;
//# sourceMappingURL=accountStore.js.map