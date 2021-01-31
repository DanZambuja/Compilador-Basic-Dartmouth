LABEL_10:
   ldr r11, =2
   ldr r12, =1
   add r0, r12, r11
   mov r12, r0
   ldr r9, =3
   ldr r10, =1
   sub r0, r10, r9
   mov r10, r0
   ldr r11, =7
   mul r0, r11, r10
   mov r11, r0
   sub r0, r12, r11
   mov r12, r0
   mov r0, r12
   ldr r1, =0
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
LABEL_20:
   ldr r1, =0
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   ldr r0, [r2]
  mov r1, r0
   ldr r0, =17
    cmp r1, r0
    beq LABEL_40
LABEL_30:
   ldr r0, =PRINT_1
   bl print_uart0
LABEL_40:
   ldr r0, =PRINT_2
   bl print_uart0
