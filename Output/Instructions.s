LABEL_10:
   ldr r11, =4
   ldr r12, =25
   add r0, r12, r11
   mov r12, r0
   ldr r11, =7
   sub r0, r12, r11
   mov r12, r0
   ldr r11, =4
   mul r0, r12, r11
   mov r12, r0
   ldr r10, =10
   ldr r11, =4
   add r0, r11, r10
   mov r11, r0
   add r0, r12, r11
   mov r12, r0
   ldr r11, =15
   add r0, r12, r11
   mov r12, r0
   ldr r9, =5
   ldr r10, =9
   mul r0, r10, r9
   mov r10, r0
   ldr r11, =3
   mul r0, r11, r10
   mov r11, r0
   mov r0, r12
   ldr r1, =0
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
