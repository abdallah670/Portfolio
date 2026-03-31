import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

interface AdminProject {
  id: string;
  name: string;
  ref: string;
  image: string;
  tags: string[];
  status: 'Published' | 'Draft';
  createdAt: string;
  updatedAt: string;
}

@Component({
  selector: 'app-admin-projects',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.scss']
})
export class ProjectsComponent {
  projects: AdminProject[] = [
    {
      id: '1',
      name: 'Nebula CRM',
      ref: 'NC-2024-001',
      image: 'https://lh3.googleusercontent.com/aida-public/AB6AXuBf_r2HBCH4zbITxXwxYLDQD2HBDrUwKZeXfqT-7Pf1kjHsQsfHIiW88IS23LokgFM4-dQhzXjuCayktdTVFwdJoHUBdoPtjz8czkZsqbO3DvJfWtthtE0IYbH2WRDrA3mroOyDhg_u24jREwMLmrxjahu8BViK7GSR52ESd-JwLFZCWGyKGkMXA1mpeMQxBjMNCPnsJSgnBip6c8yKYyZsXh2sAxEeYjjM4YFl8ooBEuVY9Ya6eQKxpqpJDKhfnVTeHUOPOBZGrUg',
      tags: ['React', 'Node.js', 'Prisma'],
      status: 'Published',
      createdAt: '24 Oct 2023',
      updatedAt: 'Last updated 2h ago'
    },
    {
      id: '2',
      name: 'Aether Engine',
      ref: 'AE-2023-094',
      image: 'https://lh3.googleusercontent.com/aida-public/AB6AXuDnpTmNz_JMT9l2npClIL6nIM-gHCkiOix5EFzKCyJ8hR9fsDuIlV-yeAXxCSDFfpEni6BFMPYRLThURNx44tlWetlLS6nNaYGIw6PmCKJ-757cG5KvLbgllFoVt76hMBHKOTarorjYZ_Smtg9PRYxIjzw-_B2EKpINnq8iRjzTqOOOJFc1gMT0rpqxG7luu32LEsd6mjxIi4-ar9jk_Vk-B0GTPcG-brehgQs1-cuz20odWFCNgGUuF0d9tgphBsx51YI2Xv6XREU',
      tags: ['TypeScript', 'WebGL', 'Rust'],
      status: 'Draft',
      createdAt: '12 Jan 2024',
      updatedAt: 'Created 3d ago'
    },
    {
      id: '3',
      name: 'Titan Mesh v3',
      ref: 'TM-2024-004',
      image: 'https://lh3.googleusercontent.com/aida-public/AB6AXuD7CS0AY9YNSZUCTrO5dz4XWgI1oSoevQy3Yz8yQ3G0zjH21cNHiYTInaqWnS3nq0TTT300o9j7bM7vinTyW5x9-tvmxCHHVLeUHcUEOqRvrr0H80Ztj1Xj8xHX9ya7TsqlGzD_XiZ2NKuQWj06ajDlPJmby42d28Vn5WoCtMYaPsOVFVSxCctoHBU-KE-N_tEDn4VZV_suSRj7z1WZhEXGrLmQzsj8RgU0k383O-OD_b36tSfYYN1wEe0CWRJRJD3ztEeEX3CtDUA',
      tags: ['Next.js', 'Go', 'Docker'],
      status: 'Published',
      createdAt: '05 Feb 2024',
      updatedAt: 'Last updated yesterday'
    }
  ];
}