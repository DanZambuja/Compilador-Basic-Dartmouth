LABEL_10:
   ldr r11, =4
   ldr r12, =25
   add r0, r12, r11
   mov r12, r0
   ldr r10, =4
   ldr r11, =7
   mul r0, r11, r10
   mov r11, r0
   sub r0, r12, r11
   mov r12, r0
   ldr r10, =10
   ldr r11, =4
   add r0, r11, r10
   mov r11, r0
   add r0, r12, r11
   mov r12, r0
   ldr r8, =5
   ldr r9, =9
   mul r0, r9, r8
   mov r9, r0
   ldr r10, =3
   mul r0, r10, r9
   mov r10, r0
   ldr r11, =15
   mul r0, r11, r10
   mov r11, r0
   add r0, r12, r11
   mov r12, r0
   mov r0, r12
   ldr r1, =0
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
LABEL_20:
   ldr r11, =4
   ldr r12, =31
   sub r0, r12, r11
   mov r12, r0
   mov r0, r12
   ldr r1, =1
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
LABEL_30:
   ldr r1, =1
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   ldr r0, [r2]
   ldr r1, =2
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
LABEL_40:
   ldr r1, =2
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   ldr r11, [r2]
   ldr r12, =3
   add r0, r12, r11
   mov r12, r0
   mov r0, r12
   ldr r1, =3
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
