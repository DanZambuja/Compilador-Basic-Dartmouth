LABEL_10:
   ldr r0, =10
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
   ldr r0, =10
    cmp r1, r0
    beq LABEL_40
LABEL_30:
   ldr r0, =PRINT_1
   bl print_uart0
LABEL_40:
   ldr r0, =PRINT_2
   bl print_uart0
