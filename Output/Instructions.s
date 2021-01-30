LABEL_10:
    b LABEL_40
LABEL_20:
   ldr r0, =PRINT_1
   bl print_uart0
LABEL_30:
   b END
LABEL_40:
   ldr r0, =PRINT_2
   bl print_uart0
LABEL_50:
   ldr r0, =1145
   ldr r1, =0
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
LABEL_60:
    b LABEL_20
