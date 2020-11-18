#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include "types.h"
#include "versatilepb_pl190_vic.h"
#include "uart.h"
#include "vid.h"

extern UART uart[4];
extern void uart_handler(UART *up);
extern int get_process_id();
extern void print_and_draw();

extern void uprints(UART *up, u8 *s);
extern void ugets(UART *up, char *s);

volatile unsigned int * const UART0DR = (unsigned int *)0x101f1000;
volatile unsigned int * const TIMER0X = (unsigned int *)0x101E200c;
u8 *p;
int nproc = 0;
int pid;
int position;
int color;

void print_uart0(const char *s) {
  while(*s != '\0') { /* Loop until end of string */
    *UART0DR = (unsigned int)(*s); /* Transmit char */
    s++; /* Next char */
  }
}

void setup_uart() {
  u8 line[128];
  UART *up;
  VIC_INTENABLE |= UART0_IRQ_VIC_BIT;
  VIC_INTENABLE |= UART1_IRQ_VIC_BIT;

  uart_init();
  up= &uart[0];
  fbuf_init();
  p = &_binary_image_bmp_start; // symbol generated by the linker based on the input file name
}

void p_d_syscall() {
  show_bmp(p, position, 0, color);
  show_bmp_black(p, position, 600);
  // print_uart0(pid);
}

int get_pid_syscall() {
  return nproc;
}

void task() {
  pid = get_process_id();
  position = pid * 60 + 1;
  color = pid * 100000 + 100000;

  while(1) {
    print_and_draw();
  }
}

void handler_timer() {
  *TIMER0X = (unsigned int)(0);
}

void IRQ_handler()
{
    u32 vicstatus = VIC_STATUS;

    //UART 0
    if(vicstatus & UART0_IRQ_VIC_BIT)
    {
      uart_handler(&uart[0]);
    }

    //UART 1
    if(vicstatus & UART1_IRQ_VIC_BIT)
    {
      uart_handler(&uart[1]);
    }

}

// void task1() {
//   // int pid = get_process_id();

//   // int position = pid * 50 + 1;
//   // int color = pid * 100000 + 100000;
//   while(1) {
//     show_bmp(p, 0, 0, 100000);
//     show_bmp_black(p, 0, 0);
//     // show_bmp(p, position, 0, color);
//     // show_bmp_black(p, position, 0);
//     print_uart0("1");
//   }
// }

// void task2() {
//   // int pid = get_process_id();

//   // int position = pid * 50 + 1;
//   // int color = pid * 100000 + 100000;
//   while(1) {
//     show_bmp(p, 0, 51, 200000);
//     show_bmp_black(p, 0, 51);
//     print_uart0("2");
//   }
// }

// void task3() {
//   int pid = get_process_id();

//   int position = pid * 50 + 1;
//   int color = pid * 100000 + 100000;
//   while(1) {
//     show_bmp(p, 0, 101, 300000);
//     show_bmp_black(p, 0, 101);
//     print_uart0("3");
//   }
// }

// void task4() {
//   int pid = get_process_id();

//   int position = pid * 50 + 1;
//   int color = pid * 100000 + 100000;
//   while(1) {
//     show_bmp(p, 0, 151, 400000);
//     show_bmp_black(p, 0, 151);
//     print_uart0("4");
//   }
// }

// void task5() {
//   int pid = get_process_id();

//   int position = pid * 50 + 1;
//   int color = pid * 100000 + 100000;
//   while(1) {
//     show_bmp(p, 0, 200, 500000);
//     show_bmp_black(p, 0, 200);
//     print_uart0("5");
//   }
// }

// void task6() {
//   int pid = get_process_id();

//   int position = pid * 50 + 1;
//   int color = pid * 100000 + 100000;
//   while(1) {
//     show_bmp(p, 100, 0, 600000);
//     show_bmp_black(p, 100, 0);
//     print_uart0("6");
//   }
// }

// void task7() {
//   int pid = get_process_id();

//   int position = pid * 50 + 1;
//   int color = pid * 100000 + 100000;
//   while(1) {
//     show_bmp(p, 100, 51, 600000);
//     show_bmp_black(p, 100, 51);
//     print_uart0("7");
//   }
// }

// void task8() {
//   int pid = get_process_id();

//   int position = pid * 50 + 1;
//   int color = pid * 100000 + 100000;
//   while(1) {
//     show_bmp(p, 100, 101, 700000);
//     show_bmp_black(p, 100, 101);
//     print_uart0("8");
//   }
// }

// void task9() {
//   int pid = get_process_id();

//   int position = pid * 50 + 1;
//   int color = pid * 100000 + 100000;
//   while(1) {
//     show_bmp(p, 100, 151, 800000);
//     show_bmp_black(p, 100, 151);
//     print_uart0("9");
//   }
// }

// void task10() {
//   int pid = get_process_id();

//   int position = pid * 50 + 1;
//   int color = pid * 100000 + 100000;
//   while(1) {
//     show_bmp(p, 100, 200, 900000);
//     show_bmp_black(p, 100, 200);
//     print_uart0("10");
//   }
// }
