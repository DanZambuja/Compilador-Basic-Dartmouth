LABEL_10:
   ldr r0, =PRINT_1
   bl print_uart0
   ldr r0, =PRINT_2
   bl print_uart0
LABEL_20:
    b LABEL_40
LABEL_30:
   ldr r0, =PRINT_3
   bl print_uart0
LABEL_40:
   ldr r0, =PRINT_4
   bl print_uart0
LABEL_50:
LABEL_60:
    b LABEL_80
LABEL_70:
   ldr r0, =PRINT_5
   bl print_uart0
LABEL_80:
   ldr r0, =PRINT_6
   bl print_uart0
