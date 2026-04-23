# Deployment Guide

## Cloudinary Configuration ✅
Your Cloudinary credentials are configured:
- **Cloud Name**: `dmyrxpvnj`
- **API Key**: `917623757227562`
- **API Secret**: `vBg_bFM5qAVStGhQWi1M4eTKD_8`

## Step 1: Deploy Backend to Render

1. **Push code to GitHub**:
   ```bash
   git add .
   git commit -m "Add deployment configuration"
   git push origin main
   ```

2. **Create Render Account**: Go to [render.com](https://render.com) and sign up

3. **Create New Web Service**:
   - Connect your GitHub repository
   - Select the `webapi/Portfolio.Api` directory
   - Runtime: `.NET`
   - Build Command: `dotnet restore && dotnet publish -c Release -o out`
   - Start Command: `dotnet out/Portfolio.Api.dll`

4. **Add Environment Variables** in Render Dashboard:
   ```
   Cloudinary__CloudName = dmyrxpvnj
   Cloudinary__ApiKey = 917623757227562
   Cloudinary__ApiSecret = vBg_bFM5qAVStGhQWi1M4eTKD_8
   Jwt__Secret = <generate-random-string>
   Admin__Username = admin
   Admin__Password = <your-secure-password>
   Admin__Email = admin@yourdomain.com
   AllowedOrigins = https://abdullahmohammed.netlify.app,https://*.netlify.app
   ```

5. **Create PostgreSQL Database**:
   - In Render Dashboard, create a new PostgreSQL database (free tier)
   - Copy the "Internal Database URL" 
   - Add as environment variable: `ConnectionStrings__DefaultConnection`

## Step 2: Deploy Frontend to Netlify

1. **Build the frontend locally first**:
   ```bash
   cd frontend
   npm install
   npm run build
   ```

2. **Deploy to Netlify**:
   - Option A: Drag & drop the `dist/frontend/browser` folder to [netlify.com](https://netlify.com)
   - Option B: Connect GitHub repo and configure:
     - Build command: `npm run build`
     - Publish directory: `dist/frontend/browser`

3. **Update API URL** (if needed):
   - After Render deploys, get your API URL (e.g., `https://portfolio-api-xxxx.onrender.com`)
   - Update `frontend/src/environments/environment.prod.ts`:
     ```typescript
     apiUrl: 'https://your-render-url/api',
     baseUrl: 'https://your-render-url'
     ```
   - Rebuild and redeploy

## Step 3: Configure Email (Optional)

For contact form to work:
1. Use Gmail App Password (not your regular password)
2. Enable 2FA on Google account
3. Generate app password at: https://myaccount.google.com/apppasswords
4. Add to Render environment variables

## URLs After Deployment

- **Backend API**: `https://portfolio-api-xxxx.onrender.com`
- **Frontend**: `https://abdullahmohammed.netlify.app`
- **Admin Panel**: `https://abdullahmohammed.netlify.app/admin`

## Troubleshooting

**Images not loading?**
- Check Cloudinary dashboard for uploaded images
- Verify `baseUrl` in `environment.prod.ts`

**CORS errors?**
- Update `AllowedOrigins` in Render environment variables
- Include your exact Netlify URL

**Database connection failed?**
- Verify PostgreSQL is created and running
- Check connection string format

## Post-Deployment Setup

1. **Login to Admin Panel**:
   - URL: `https://abdullahmohammed.netlify.app/admin/login`
   - Use credentials set in `Admin__Username` and `Admin__Password`

2. **Configure Portfolio**:
   - Upload CV (goes to Cloudinary)
   - Upload profile image (goes to Cloudinary)
   - Add projects with images (goes to Cloudinary)

3. **Test Contact Form**:
   - Submit a message on the public site
   - Check it appears in admin messages

---

**You're ready to deploy!** 🚀
