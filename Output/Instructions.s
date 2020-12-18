LABEL_10:
   ldr r0, =PRINT_1
   bl print_uart0
   ldr r0, =PRINT_2
   bl print_uart0
LABEL_20:
    ldr r0, =35
   ldr r1, =9
   add r0, r0, r1
   ldr r1, =35
   sub r0, r0, r1
   ldr r1, =0
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
